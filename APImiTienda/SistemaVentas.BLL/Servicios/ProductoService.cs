﻿using System;
using System.Collections.Generic;
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
    public class ProductoService : IProductoService
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Producto> _productoRepositorio;

        public ProductoService(IMapper mapper, IGenericRepository<Producto> productoRepositorio)
        {
            _mapper = mapper;
            _productoRepositorio = productoRepositorio;
        }

        public async Task<List<ProductoDTO>> Lista()
        {
            try
            {
                var queryProducto = await _productoRepositorio.Consultar();

                var listaProducto = queryProducto.Include(cat => cat.IdCategoriaNavigation).ToList();

                return _mapper.Map<List<ProductoDTO>>(listaProducto).ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var producto_creado = await _productoRepositorio.Crear(_mapper.Map<Producto>(modelo));

                if (producto_creado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear");

                return _mapper.Map<ProductoDTO>(producto_creado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var producto_modelo = _mapper.Map<Producto>(modelo);

                var producto_encontrado = await _productoRepositorio.Obtener(u =>
                        u.IdProducto == producto_modelo.IdProducto);

                if (producto_encontrado == null)
                    throw new TaskCanceledException("El producto no existe");

                producto_encontrado.Nombre = producto_modelo.Nombre;
                producto_encontrado.IdCategoria = producto_modelo.IdCategoria;
                producto_encontrado.Stock = producto_modelo.Stock;
                producto_encontrado.Precio = producto_modelo.Precio;
                producto_encontrado.EsActivo = producto_modelo.EsActivo;

                bool respuesta = await _productoRepositorio.Editar(producto_encontrado);

                if(!respuesta)
                    throw new TaskCanceledException("No se pudo editar");

                return respuesta;

            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var producto_encontrado = await _productoRepositorio.Obtener(p => p.IdProducto == id);
                
                if(producto_encontrado == null)
                    throw new TaskCanceledException("El producto no existe");

                bool respuesta = await _productoRepositorio.Eliminar(producto_encontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo eliminar");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }
      
    }
}
