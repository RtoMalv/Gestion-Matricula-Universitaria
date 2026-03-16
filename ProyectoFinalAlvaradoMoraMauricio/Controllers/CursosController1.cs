using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinalAlvaradoMoraMauricio.Controllers
{
    public class CursosController1 : Controller
    {
        // GET: CursosController1
        public ActionResult Index()
        {
            return View();
        }

        // GET: CursosController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CursosController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CursosController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CursosController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CursosController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CursosController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CursosController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
