﻿using AspectCore.DynamicProxy;
using Destiny.Core.Flow.AOP;
using Destiny.Core.Flow.Dtos.Identitys;
using Destiny.Core.Flow.Ui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Destiny.Core.Flow.IServices.Identity
{
    /// <summary>
    /// 身份接口
    /// </summary>

    public interface IIdentityServices
    {
        //[ServiceInterceptor(typeof(TransactionalAttribute))]
        Task<OperationResponse> Login(LoginDto loginDto);
    }
}
