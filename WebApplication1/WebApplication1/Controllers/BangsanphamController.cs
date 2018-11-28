using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Transactions;
namespace WebApplication1.Controllers
{
    public class BangsanphamController : Controller
    {
        private CS4PEEntities db = new CS4PEEntities();

        // GET: /Bangsanpham/
        public ActionResult Index()
        {
            var model = db.BangSanPhams.ToList();
            return View(model);
        }

        // GET: /Bangsanpham/Details/5
        public ActionResult Details(int id)
        {
            var model = db.BangSanPhams.Find(id);
            if (model == null)
            {
                return HttpNotFound(); ;
            }
            
            return View(model);
        }

        // GET: /Bangsanpham/Create
        public ActionResult Create()
        {
            ViewBag.Loai_id = new SelectList(db.LoaiSanPhams, "id", "TenLoai");
            return View();
        }

        // POST: /Bangsanpham/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( BangSanPham model)
        {
            //Tao điều kiện
            CheckBangSanPham(model);
            if (ModelState.IsValid)
            {
                using(var scope = new TransactionScope())
                {
                db.BangSanPhams.Add(model);
                db.SaveChanges();

                var path = Server.MapPath("~/App_Data");
                path = path + "/" + model.id;
                if (Request.Files["HinhAnh"] != null && Request.Files["HinhAnh"].ContentLength > 0)
                {
                    Request.Files["HinhAnh"].SaveAs(path);
                    scope.Complete();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
                }
            }

            ViewBag.Loai_id = new SelectList(db.LoaiSanPhams, "id", "TenLoai", model.Loai_id);
            return View(model);
        }
        private void CheckBangSanPham(BangSanPham model)
        {
            if (model.GiaGoc < 0)
            {
                ModelState.AddModelError("GiaGoc", "Giá gốc phải lớn hơn 0");
            }
            if (model.GiaBan < model.GiaGoc)
            {
                ModelState.AddModelError("GiaGoc", "Giá gốc phải bé hơn giá bán");
            }
        }

        // GET: /Bangsanpham/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BangSanPham bangsanpham = db.BangSanPhams.Find(id);
            if (bangsanpham == null)
            {
                return HttpNotFound();
            }
            ViewBag.Loai_id = new SelectList(db.LoaiSanPhams, "id", "TenLoai", bangsanpham.Loai_id);
            return View(bangsanpham);
        }

        // POST: /Bangsanpham/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,MaSP,TenSP,Loai_id,GiaBan,GiaGoc,GiaGop,SoLuongTon")] BangSanPham bangsanpham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bangsanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Loai_id = new SelectList(db.LoaiSanPhams, "id", "TenLoai", bangsanpham.Loai_id);
            return View(bangsanpham);
        }

        // GET: /Bangsanpham/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BangSanPham bangsanpham = db.BangSanPhams.Find(id);
            if (bangsanpham == null)
            {
                return HttpNotFound();
            }
            return View(bangsanpham);
        }

        // POST: /Bangsanpham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BangSanPham bangsanpham = db.BangSanPhams.Find(id);
            db.BangSanPhams.Remove(bangsanpham);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}