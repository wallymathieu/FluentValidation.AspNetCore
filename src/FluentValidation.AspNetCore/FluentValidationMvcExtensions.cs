#region License
// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.AspNetCore;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class FluentValidationMvcExtensions {
	/// <summary>
	///     Adds Fluent Validation services to the specified
	///     <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.
	/// </summary>
	/// <returns>
	///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcCoreBuilder" /> that can be used to further configure the
	///     MVC services.
	/// </returns>
	/// <remarks>
	///		Calling AddFluentValidation() is deprecated. Call <c>services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()</c> instead, which has the same effect. For details see <see href="https://github.com/FluentValidation/FluentValidation/issues/1965"/>.
	/// </remarks>
	[Obsolete("Calling AddFluentValidation() is deprecated. Call services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters() instead, which has the same effect. For details see https://github.com/FluentValidation/FluentValidation/issues/1965")]
	public static IMvcCoreBuilder AddFluentValidation(this IMvcCoreBuilder mvcBuilder, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
		mvcBuilder.Services.AddFluentValidation(configurationExpression);
		return mvcBuilder;
	}

	/// <summary>
	///     Adds Fluent Validation services to the specified
	///     <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.
	/// </summary>
	/// <returns>
	///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" /> that can be used to further configure the
	///     MVC services.
	/// </returns>
	/// <remarks>
	///		Calling AddFluentValidation() is deprecated. Call <c>services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()</c> instead, which has the same effect. For details see <see href="https://github.com/FluentValidation/FluentValidation/issues/1965"/>.
	/// </remarks>
	[Obsolete("Calling AddFluentValidation() is deprecated. Call services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters() instead, which has the same effect. For details see https://github.com/FluentValidation/FluentValidation/issues/1965")]
	public static IMvcBuilder AddFluentValidation(this IMvcBuilder mvcBuilder, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
		mvcBuilder.Services.AddFluentValidation(configurationExpression);
		return mvcBuilder;
	}

#pragma warning disable CS0618
	/// <summary>
	///     Adds Fluent Validation services to the specified
	///     <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
	/// </summary>
	/// <returns>
	///     A reference to this instance after the operation has completed.
	/// </returns>
	/// <remarks>
	///		Calling AddFluentValidation() is deprecated. Call <c>services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()</c> instead, which has the same effect. For details see <see href="https://github.com/FluentValidation/FluentValidation/issues/1965"/>.
	/// </remarks>
	[Obsolete("Calling AddFluentValidation() is deprecated. Call services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters() instead, which has the same effect. For details see https://github.com/FluentValidation/FluentValidation/issues/1965")]
	public static IServiceCollection AddFluentValidation(this IServiceCollection services, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
		var config = new FluentValidationMvcConfiguration(ValidatorOptions.Global, services);
		configurationExpression?.Invoke(config);

		services.AddSingleton(config.ValidatorOptions);

		if (config.AutomaticValidationEnabled) {
			services.AddFluentValidationAutoValidation(cfg => {
			});
		}

		return services;
	}

	/// <summary>
	/// Enables integration between FluentValidation and ASP.NET MVC's automatic validation pipeline.
	/// </summary>
	/// <param name="services">services</param>
	/// <param name="configurationExpression">Configuration callback</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddFluentValidationAutoValidation(this IServiceCollection services, Action<FluentValidationAutoValidationConfiguration> configurationExpression = null) {
		var config = new FluentValidationAutoValidationConfiguration();
		configurationExpression?.Invoke(config);

		services.TryAddSingleton(ValidatorOptions.Global);

		services.Add(ServiceDescriptor.Scoped(typeof(IValidatorFactory), typeof(ServiceProviderValidatorFactory)));
		

		services.Add(ServiceDescriptor.Singleton<IObjectModelValidator, FluentValidationObjectModelValidator>(s => {
			var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
			var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
			return new FluentValidationObjectModelValidator(metadataProvider, options.ModelValidatorProviders, true);
		}));

		services.Configure<MvcOptions>(options => {
			// Check if the providers have already been added.
			// We shouldn't have to do this, but there's a bug in the ASP.NET Core integration
			// testing components that can cause Configureservices to be called multple times
			// meaning we end up with duplicates.

			if (!options.ModelMetadataDetailsProviders.Any(x => x is FluentValidationBindingMetadataProvider)) {
				options.ModelMetadataDetailsProviders.Add(new FluentValidationBindingMetadataProvider());
			}

			if (!options.ModelValidatorProviders.Any(x => x is FluentValidationModelValidatorProvider)) {
				options.ModelValidatorProviders.Insert(0, new FluentValidationModelValidatorProvider(
					implicitValidationEnabled: false,
					implicitRootCollectionElementValidationEnabled: false,
					filter: config.Filter));
			}
		});

		return services;
	}

#pragma warning restore CS0618

}

