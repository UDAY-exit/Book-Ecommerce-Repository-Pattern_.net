using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using Ecommerce12Aug_Project.Models;
using Ecommerce12Aug_Project.Models.ViewModels;
using Ecommerce12Aug_Project.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Ecommerce12Aug_Project.Areas.Coustomer.Controllers
{
    [Area("Coustomer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly TwilioService _twilioService;

        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(IUnitOfWork unitOfWork,IEmailSender emailSender,UserManager<ApplicationUser> userManager,TwilioService twilioService)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _twilioService = twilioService;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll
                (sc=>sc.ApplicationUserId == claims.Value,includeProperties:"Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
                    list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
                }
            }

            //Email Confirm

            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent kindly verify your email !";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must be confirm for authorize customer !";
                ViewBag.EmailCSS = "text-danger";
            }

            //**

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            if (user == null)
                ModelState.AddModelError(string.Empty, "Email Empty !!!!");
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme)!;

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.Get(id);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.Get(id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.Get(id);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product")
                ,
                OrderHeader = new OrderHeader()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.
                FirstOrDefault(au => au.Id == claims.Value);

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count,
                    list.Product.Price, list.Product.Price50, list.Product.Price100);

                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if(list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0,99)+"....";
                }
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.Cty;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            return View(ShoppingCartVM);
        }

        //[HttpPost]
        //[ActionName("summary")]
        //public IActionResult summaryPost(string stripeToken)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    if (claims == null) return NotFound();

        //    ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value, includeProperties: "Product");

        //    ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);

        //    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
        //    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        //    ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;
        //    _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        //    _unitOfWork.Save();

        //    foreach(var list in ShoppingCartVM.ListCart)
        //    {
        //        list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
        //        ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
        //        OrderDetail orderDetail = new OrderDetail()
        //        {
        //            OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
        //            ProductId = list.ProductId,
        //            Price = list.Price,
        //            Count = list.Count
        //        };
        //        _unitOfWork.OrderDetail.Add(orderDetail);
        //        _unitOfWork.Save();
        //    }
        //    // remove from shoppingcart model
        //    _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
        //    _unitOfWork.Save();

        //    //session count
        //    HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).Count());

        //    //Stripe Payment

        //    if(stripeToken == null)
        //    {
        //        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
        //        ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        //        ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;

        //    }
        //    else
        //    {
        //        var options = new ChargeCreateOptions()
        //        {
        //            Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
        //            Currency = "usd",
        //            Description = "Order Id :" + ShoppingCartVM.OrderHeader.Id.ToString(),
        //            Source = stripeToken
        //        };
        //        var service = new ChargeService();
        //        Charge charge = service.Create(options);
        //        if (charge.BalanceTransactionId == null) ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
        //        else

        //            ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
        //        if(charge.Status.ToLower()== "succeeded")
        //        {
        //            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
        //            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
        //            //ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        //            ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;

        //        }
        //        _unitOfWork.Save();
        //    }

        //    return RedirectToAction("OrderConfirmation", "Cart",
        //        new { id = ShoppingCartVM.OrderHeader.Id });
        //}
        [HttpPost]
        [ActionName("summary")]
        public async Task<IActionResult> summaryPost(string paymentMethodId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims == null)
                return NotFound();

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(
                sc => sc.ApplicationUserId == claims.Value,
                includeProperties: "Product"
            );

            ShoppingCartVM.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.FirstOrDefault(
                    au => au.Id == claims.Value
                );

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(
                    list.Count,
                    list.Product.Price,
                    list.Product.Price50,
                    list.Product.Price100
                );

                ShoppingCartVM.OrderHeader.OrderTotal +=
                    (list.Price * list.Count);

                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = list.ProductId,
                    Price = list.Price,
                    Count = list.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            // Remove from shopping cart
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();

            // Session count
            HttpContext.Session.SetInt32(
                SD.Ss_CartSessionCount,
                _unitOfWork.ShoppingCart
                    .GetAll(sc => sc.ApplicationUserId == claims.Value)
                    .Count()
            );

            // Stripe Payment
            if (string.IsNullOrEmpty(paymentMethodId))
            {
                ShoppingCartVM.OrderHeader.PaymentStatus =
                    SD.PaymentStatusDelayPayment;

                ShoppingCartVM.OrderHeader.PaymentDueDate =
                    DateTime.Now.AddDays(30);

                ShoppingCartVM.OrderHeader.OrderStatus =
                    SD.OrderStatusApproved;
            }
            else
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt64(
                        ShoppingCartVM.OrderHeader.OrderTotal * 100
                    ),
                    Currency = "usd",
                    Description = "Order Id : " +
                                  ShoppingCartVM.OrderHeader.Id,

                    PaymentMethod = paymentMethodId,
                    Confirm = true
                };

                var service = new PaymentIntentService();

                PaymentIntent paymentIntent =
                    service.Create(options);

                if (paymentIntent.Status == "succeeded")
                {
                    ShoppingCartVM.OrderHeader.OrderStatus =
                        SD.OrderStatusApproved;

                    ShoppingCartVM.OrderHeader.PaymentStatus =
                        SD.PaymentStatusApproved;

                    ShoppingCartVM.OrderHeader.PaymentDate =
                        DateTime.Now;

                    ShoppingCartVM.OrderHeader.TransactionId =
                        paymentIntent.LatestChargeId;
                }
                else
                {
                    ShoppingCartVM.OrderHeader.PaymentStatus =
                        SD.PaymentStatusRejected;
                }

                _unitOfWork.Save();
            }
            // Send Confirmation Email, SMS & Call
            if (ShoppingCartVM.OrderHeader.OrderStatus == SD.OrderStatusApproved)
            {
                var customerUser = ShoppingCartVM.OrderHeader.ApplicationUser
                                   ?? _unitOfWork.ApplicationUser
                                       .FirstOrDefault(u => u.Id == claims.Value);

                // 1. Send Email
                if (!string.IsNullOrEmpty(customerUser?.Email))
                {
                    await _emailSender.SendEmailAsync(
                        customerUser.Email,
                        $"Order Confirmation - #{ShoppingCartVM.OrderHeader.Id}",
                        $"<p>Your order <strong>#{ShoppingCartVM.OrderHeader.Id}</strong> has been confirmed successfully. Thank you for shopping with us!</p>"
                    );
                }

                // 2. Send SMS + Make Call
                if (!string.IsNullOrWhiteSpace(customerUser?.PhoneNumber))
                {
                    string phone = customerUser.PhoneNumber;

                    if (!phone.StartsWith("+91"))
                    {
                        phone = "+91" + phone;
                    }

                    // SMS
                    _twilioService.SendOrderSms(
                        phone,
                        ShoppingCartVM.OrderHeader.Id
                    );

                    // Voice Call
                    _twilioService.MakeOrderCall(
                        phone,
                        ShoppingCartVM.OrderHeader.Id
                    );
                }
            }

            // Redirect to Order Confirmation
            return RedirectToAction(
                "OrderConfirmation",
                "Cart",
                new
                {
                    id = ShoppingCartVM.OrderHeader.Id
                }
            );
        }


        // =========================================================
        // TWILIO ORDER CALL
        // =========================================================

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public IActionResult OrderCall(int orderId)
        {
            var response = new Twilio.TwiML.VoiceResponse();

            var gather = new Twilio.TwiML.Voice.Gather(
                numDigits: 1,
                action: new Uri(
                    "https://broken-prologue-brilliant.ngrok-free.dev/Coustomer/Cart/OrderCallResponse?orderId="
                    + orderId
                ),
                method: Twilio.Http.HttpMethod.Post
            );

            gather.Say(
                "Hello. Your order number " + orderId +
                " has been placed. " +
                "Press 1 to confirm your order. " +
                "Press 2 to cancel your order."
            );

            response.Append(gather);

            return Content(
                response.ToString(),
                "application/xml"
            );
        }


        // =========================================================
        // TWILIO ORDER CALL RESPONSE
        // =========================================================

        [AllowAnonymous]
        [HttpPost]
        public IActionResult OrderCallResponse(
            int orderId,
            string Digits)
        {
            var order = _unitOfWork.OrderHeader.Get(orderId);

            if (order == null)
                return NotFound();

            var response = new Twilio.TwiML.VoiceResponse();

            if (Digits == "1")
            {
                order.OrderStatus = SD.OrderStatusApproved;

                _unitOfWork.Save();

                response.Say(
                    "Thank you. Your order has been confirmed. Goodbye."
                );
            }
            else if (Digits == "2")
            {
                order.OrderStatus = SD.OrderStatusCancelled;

                _unitOfWork.Save();

                response.Say(
                    "Your order has been cancelled. Goodbye."
                );
            }
            else
            {
                response.Say(
                    "Sorry, that was not a valid choice. Goodbye."
                );
            }

            return Content(
                response.ToString(),
                "application/xml"
            );
        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
