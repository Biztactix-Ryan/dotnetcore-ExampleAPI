using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExpress.Xpo;
using ExampleAPI.Contracts.Shared;
using ExampleAPI.Contracts.V1;
using ExampleAPI.Models.ExampleXPOModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExampleAPI.Controllers.V1
{
    
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private UnitOfWork _uow;
        public ExampleController(IMapper mapper,UnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }
        // GET: api/<ExampleController>
        [HttpGet(APIRoutes.Example.Route)]
        public IEnumerable<ExampleObjectResponse> Get()
        {
            var ExObjs = _uow.Query<ExampleObject>();
            var objs = _mapper.Map<List<ExampleObjectResponse>>(ExObjs);
            return objs;
            
        }

        // GET api/<ExampleController>/5
        [HttpGet(APIRoutes.Example.RoutebyId)]
        public ExampleObjectResponse Get(int id)
        {
            var ExObj = _uow.GetObjectByKey<ExampleObject>(id);
            return _mapper.Map<ExampleObjectResponse>(ExObj);
        }

        // POST api/<ExampleController>
        [HttpPost(APIRoutes.Example.Route)]
        public ExampleObjectResponse Post([FromForm] ExampleObjectCreate value)
        {
            ExampleObject Obj = new ExampleObject(_uow);
            _mapper.Map(value,Obj);
            Obj.Save();
            _uow.CommitChanges();
            return _mapper.Map<ExampleObjectResponse>(Obj);
        }

        // POST api/<ExampleController>
        [HttpPut(APIRoutes.Example.RoutebyId)]
        public async Task<IActionResult> Put([FromRoute] int id,[FromForm] ExampleObjectUpdate value)
        {
            ExampleObject Obj = _uow.GetObjectByKey<ExampleObject>(id);
            if(Obj == null) { return BadRequest(new ErrorResponse()); }
            _mapper.Map(value, Obj);
            Obj.Save();
            _uow.CommitChanges();
            return Ok(_mapper.Map<ExampleObjectResponse>(Obj));
        }

    }
}
