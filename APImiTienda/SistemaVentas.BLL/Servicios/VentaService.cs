using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.BLL;
using SistemaVentas.BLL.Servicios.Contrato;
using SistemaVentas.DAL.Repositorios.Contrato;
using SistemaVentas.DTO;
using SistemaVentas.Model;

namespace SistemaVentas.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IVentaRepository _ventaRepositorio;

        public VentaService(
            IMapper mapper, 
            IGenericRepository<DetalleVenta> detalleVentaRepositorio, 
            IVentaRepository ventaRepositorio)
        {
            _mapper = mapper;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _ventaRepositorio = ventaRepositorio;
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var venta_generada = await _ventaRepositorio.Registrar(_mapper.Map<Venta>(modelo));

                if (venta_generada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<VentaDTO>(venta_generada);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();

            var ListaResultado = new List<Venta>();

            try
            {
                if(buscarPor == "fecha")
                {
                    DateTime fecha_inicio = DateTime.ParseExact(fechaInicio,"dd/MM/yyyy", new CultureInfo("es-NIC"));
                    DateTime fecha_fin = DateTime.ParseExact(fechaFin,"dd/MM/yyyy", new CultureInfo("es-NIC"));

                    ListaResultado = await query.Where(v =>
                            v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                            v.FechaRegistro.Value.Date <= fecha_fin.Date
                         ).Include(dv => dv.DetalleVenta)
                         .ThenInclude(p => p.IdProductoNavigation)
                         .ToListAsync();
                }
                else
                {
                     ListaResultado = await query.Where(v => v.NumeroDocumento ==  numeroVenta
                         ).Include(dv => dv.DetalleVenta)
                         .ThenInclude(p => p.IdProductoNavigation)
                         .ToListAsync();
                }
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<VentaDTO>>(ListaResultado);
        }

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {

            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
            var listaResultado = new List<DetalleVenta>();

            try
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-NIC"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-NIC"));

                listaResultado = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => 
                        dv.IdVentaNavigation.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                        dv.IdVentaNavigation.FechaRegistro.Value.Date <= fecha_fin.Date
                       ).ToListAsync();
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<ReporteDTO>>(listaResultado);
        }
    }
}
