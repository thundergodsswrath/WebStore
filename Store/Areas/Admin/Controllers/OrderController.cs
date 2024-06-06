using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;
using Stripe;

namespace Store.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public OrderVM OrderVM { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        OrderVM = new()
        {
            OrderHeader =
                _unitOfWork.OrderHeaderRepository.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
            OrderDetail =
                _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
        };

        return View(OrderVM);
    }

    [HttpPost]
    [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);
        orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderVM.OrderHeader.City;
        orderHeaderFromDb.State = OrderVM.OrderHeader.State;
        orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
        {
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
        }

        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
        {
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
        }

        _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
        _unitOfWork.Save();


        return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
    }
    
    [HttpPost]
    [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
    public IActionResult StartProcessing() {
        _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }
    
    [HttpPost]
    [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
    public IActionResult ShipOrder() {

        var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);
        orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
        orderHeader.OrderStatus = StaticDetails.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;

        _unitOfWork.OrderHeaderRepository.Update(orderHeader);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }
    
    [HttpPost]
    [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
    public IActionResult CancelOrder() {

        var orderHeader = _unitOfWork.OrderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);

        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved) {
            var options = new RefundCreateOptions {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, StaticDetails.StatusCanceled, StaticDetails.StatusRefunded);
        }
        else {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, StaticDetails.StatusCanceled, StaticDetails.StatusCanceled);
        }
        _unitOfWork.Save();
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

    }


    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> objOrderHeaders =
            _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();

        if (User.IsInRole(StaticDetails.RoleAdmin) || User.IsInRole(StaticDetails.RoleEmployee))
        {
            objOrderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            objOrderHeaders = _unitOfWork.OrderHeaderRepository
                .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }

        switch (status)
        {
            case "pending":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusPending);
                break;
            case "inprocess":
                objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
                break;
            case "completed":
                objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusShipped);
                break;
            case "approved":
                objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
                break;
            default:
                break;
        }

        return Json(new { data = objOrderHeaders });
    }
}