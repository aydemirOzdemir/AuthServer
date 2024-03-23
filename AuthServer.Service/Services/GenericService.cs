using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitofWork;
using AuthServer.Service.Mappings;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IGenericRepository<TEntity> repository;
        private readonly IUnitofWork unitofWork;

        public GenericService(IGenericRepository<TEntity> repository,IUnitofWork unitofWork)
        {
            this.repository = repository;
            this.unitofWork = unitofWork;
        }
        public async Task<ResponseDto<TDto>> Add(TDto dto)
        {
            var entity = ObjectMapper.Mapper.Map<TEntity>(dto);
            await repository.Add(entity);
            await unitofWork.CommitAsync();
            var newDto=ObjectMapper.Mapper.Map<TDto>(entity);
            return ResponseDto<TDto>.Success(newDto,200);
        }

        public async Task<ResponseDto<IEnumerable<TDto>>> GetAllAsync()
        {
            var products=ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await repository.GetAllAsync());
            return ResponseDto<IEnumerable<TDto>>.Success(products,200);
        }

        public async Task<ResponseDto<TDto>> GetByIdAsync(int id)
        {
            var product = await repository.GetByIdAsync(id);
                if (product == null)
                return ResponseDto<TDto>.Fail("Id not Found", 404, true);
                
            return ResponseDto<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(product),200);
        }

        public async Task<ResponseDto<NoDataDto>> Remove(int id)
        {
            var product=await repository.GetByIdAsync(id);
            if (product == null)
                return ResponseDto<NoDataDto>.Fail("Bu Id ile silinecek nesne bulunamadı",404,true);
            repository.Remove(product);
            unitofWork.Commit();
            return ResponseDto<NoDataDto>.Success(200);
        }

        public async Task<ResponseDto<NoDataDto>> Update(TDto dto,int id)
        {
            var product = await repository.GetByIdAsync(id);
            if (product == null)
                return ResponseDto<NoDataDto>.Fail("Bu Id ile güncellenecek nesne bulunamadı", 404, true);
            repository.Update(product);
            unitofWork.Commit();
            return ResponseDto<NoDataDto>.Success(204);

        }

        public async Task<ResponseDto<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list=repository.Where(predicate);
            return ResponseDto<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>( list.ToList()), 200);
        }
    }
}
