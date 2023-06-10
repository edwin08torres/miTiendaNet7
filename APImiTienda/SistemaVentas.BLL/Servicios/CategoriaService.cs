using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVentas.BLL.Servicios.Contrato;
using SistemaVentas.DAL.Repositorios.Contrato;
using SistemaVentas.DTO;
using SistemaVentas.Model;

namespace SistemaVentas.BLL.Servicios
{
    public class CategoriaService : ICategoriaService
    {

        private readonly IMapper _mapper;
        private readonly IGenericRepository<Categoria> _categoriaRepositorio;

        public CategoriaService(IMapper mapper, IGenericRepository<Categoria> categoriaRepositorio)
        {
            _mapper = mapper;
            _categoriaRepositorio = categoriaRepositorio;
        }

        public async Task<List<CategoriaDTO>> Lista()
        {
            try
            {
                var lista_categorias = await _categoriaRepositorio.Consultar();

                return _mapper.Map<List<CategoriaDTO>>(lista_categorias).ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
