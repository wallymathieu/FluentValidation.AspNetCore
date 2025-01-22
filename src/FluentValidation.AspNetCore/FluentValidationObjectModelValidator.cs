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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

internal class FluentValidationObjectModelValidator : ObjectModelValidator {
	public FluentValidationObjectModelValidator(
		IModelMetadataProvider modelMetadataProvider,
		IList<IModelValidatorProvider> validatorProviders)
		: base(modelMetadataProvider, validatorProviders) {
	}

	public override ValidationVisitor GetValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState) {
		// Setting as to whether we should run only FV or FV + the other validator providers
		var validatorProviderToUse = validatorProvider;

		var visitor = new FluentValidationVisitor(
			actionContext,
			validatorProviderToUse,
			validatorCache,
			metadataProvider,
			validationState);

		return visitor;
	}
}
