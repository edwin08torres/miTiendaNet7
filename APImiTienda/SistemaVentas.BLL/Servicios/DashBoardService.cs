﻿using SistemaVentas.BLL.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVentas.BLL;
using SistemaVentas.DAL.Repositorios.Contrato;
using SistemaVentas.Model;
using System.Globalization;
using SistemaVentas.DTO;

namespace SistemaVentas.BLL.Servicios
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Producto> _productoRepositorio;
        private readonly IVentaRepository _ventaRepositorio;

        public DashBoardService(
            IMapper mapper, 
            IGenericRepository<Producto> productoRepositorio, 
            IVentaRepository ventaRepositorio)
        {
            _mapper = mapper;
            _productoRepositorio = productoRepositorio;
            _ventaRepositorio = ventaRepositorio;
        }

        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVenta, int restarCantidadDias)
        {
            DateTime? ultimaFecha = tablaVenta.OrderByDescending(v => v.FechaRegistro).Select(v => v.FechaRegistro).First();

            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);

            return tablaVenta.Where(v => v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }

        private async Task<int> TotalVentaUltimaSemana()
        {
            int total = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if(_ventaQuery.Count() > 0)
            {
                var tableVneta = retornarVentas(_ventaQuery, -7);
                total = tableVneta.Count();
            }

            return total;
        }

        private async Task<string> TotalIngresosUltimaSemana()
        {
            decimal resultado = 0;
            IQueryable<Venta> ventaQuery = await _ventaRepositorio.Consultar();

            if(ventaQuery.Count() > 0)
            {
                var tablaventa = retornarVentas(ventaQuery, -7);

                resultado = tablaventa.Select(v => v.Total).Sum(v => v.Value);
            }
        
              return Convert.ToString(resultado, new CultureInfo("es-NIC"));
        }

        private async Task<int> TotalProductos()
        {
            IQueryable<Producto> _productoQuery = await _productoRepositorio.Consultar();

            int total = _productoQuery.Count();

            return total;
        }

        private async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if(_ventaQuery.Count() > 0)
            {
                var tablaventa =  retornarVentas(_ventaQuery, -7);

                resultado = tablaventa
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
            }

            return resultado;
        }

        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDashBoard = new DashBoardDTO();

            try
            {
                vmDashBoard.TotalVenta = await TotalVentaUltimaSemana();
                vmDashBoard.TotalIngresos = await TotalIngresosUltimaSemana();
                vmDashBoard.TotalProductos = await TotalProductos();

                List<VentaSemanaDTO> listaVentaSemana = new List<VentaSemanaDTO>();
                
                foreach(KeyValuePair<string, int> item in await VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VentaSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }

                vmDashBoard.VentaUltimaSemana = listaVentaSemana;
                vmDashBoard.VentaUltimaSemana = listaVentaSemana;

            }
            catch
            {
                throw;
            }
            return vmDashBoard; 
        }
    }
}