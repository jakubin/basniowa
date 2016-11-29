using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Website.Infrastructure.ErrorHandling;
using Xunit;

namespace Tests.Infrastructure.ErrorHandling
{
    public class HttpErrorFilterTests
    {
        [Fact(DisplayName = nameof(HttpErrorFilter) + ": On HttpErrorException the filter should set the response from exception.")]
        public void OnHttpErrorException()
        {
            var filter = new HttpErrorFilter();
            var result = new StatusCodeResult(500);
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var context = new ExceptionContext(actionContext, new[] { filter })
            {
                Exception = new HttpErrorException(result)
            };

            filter.OnException(context);

            context.Result.Should().BeSameAs(result);
        }

        [Fact(DisplayName = nameof(HttpErrorFilter) + ": On exception other than HttpErrorException the filter should not take any actions.")]
        public void OnOtherException()
        {
            var filter = new HttpErrorFilter();
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var context = new ExceptionContext(actionContext, new[] { filter })
            {
                Exception = new InvalidOperationException()
            };

            filter.OnException(context);

            context.Result.Should().BeNull();
            context.ExceptionHandled.Should().BeFalse();
        }
    }
}
