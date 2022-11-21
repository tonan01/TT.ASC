using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TT.DoAn.Models;
using System.Web.Services.Description;
using System.Data.Entity.Infrastructure;

namespace TT.DoAn.Controllers
{
    public class HomeController : Controller
    {
        ThucTap_DoAnDataContext db = new ThucTap_DoAnDataContext();

        public ActionResult Home()
        {
            if (Session["sinhvien"] == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        #region Đăng Nhập
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XuLyDangNhap(FormCollection form)
        {
            if (form["mssv"] != "" && form["matkhau"] != "")
            {
                string maSV = form["mssv"].ToString();
                string matKhau = form["matkhau"].ToString();
                var sinhVien = db.sp_GetSinhVien(maSV, matKhau).FirstOrDefault();
                if (sinhVien != null)
                {
                    //lưu sinh viên
                    Session["sinhvien"] = sinhVien;
                    //Trả về trang mấy ông muốn sau khi đăng nhập thành công!
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    ViewBag.Loi = "Mã số sinh viên/mật khẩu không chính xác!";
                }
            }
            else
            {
                ViewBag.Loi = "Mã số sinh viên/mật khẩu không được để trống";
            }
            return View("DangNhap");
        }
        #endregion

        #region Đăng xuất
        public ActionResult DangXuat()
        {
            Session["sinhvien"] = null;
            return RedirectToAction("DangNhap", "Home");
        }
        #endregion

        #region Quản lý phiếu thu

        public ActionResult GetDanhSachPhieuThu()
        {
            if (Session["sinhvien"] == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }
            return View();

        }
        //json phiếu thu
        public JsonResult Read_DanhSachPhieuThu(string pTrangThai, [DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.sp_DanhSachQuanLyThu(pTrangThai).ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //json chi tiết phiếu thu
        public ActionResult Read_DanhSachChiTietPhieuThu(string pSoPhieu, [DataSourceRequest] DataSourceRequest request)
        {
            return Json(db.sp_ChiTietPhieuThu(pSoPhieu).ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //Delete
        public ActionResult DeletePhieuThu(string pSoPhieu, [DataSourceRequest] DataSourceRequest request)
        {

            if (pSoPhieu == null)
            {
                return HttpNotFound();
            }

            try
            {
                db.sp_DeletePhieuThu(pSoPhieu);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("GetDanhSachPhieuThu", "Home");
            #endregion
        }
    }
}