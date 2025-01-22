namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using AspNetCore;
	using Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.RazorPages;

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModel : PageModel {

		[BindProperty]
		public TestModel Test { get; set; }

		public Task<IActionResult> OnPostAsync() {
			return Task.FromResult(TestResult());
		}

		private IActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
				}
			}

			return new JsonResult(errors);
		}
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class RulesetTestPageModel : PageModel {

		[BindProperty]
		[CustomizeValidator(RuleSet = "Names")]
		public RulesetTestModel Test { get; set; }

		public Task<IActionResult> OnPostAsync() {
			return Task.FromResult(TestResult());
		}

		private IActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
				}
			}

			return new JsonResult(errors);
		}
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithPrefix : PageModel {

		[BindProperty(Name = "Test")]
		public TestModel Test { get; set; }

		public Task<IActionResult> OnPostAsync() {
			return Task.FromResult(TestResult());
		}

		private IActionResult TestResult() {
			var errors = new List<SimpleError>();

			foreach (var pair in ModelState) {
				foreach (var error in pair.Value.Errors) {
					errors.Add(new SimpleError { Name = pair.Key, Message = error.ErrorMessage });
				}
			}

			return new JsonResult(errors);
		}
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithDefaultRuleSet : PageModel {

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithSpecifiedRuleSet : PageModel {

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithMultipleRuleSets : PageModel {

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithDefaultAndSpecifiedRuleSet : PageModel {

		public IActionResult OnGet() => Page();
	}

	[IgnoreAntiforgeryToken(Order = 1001)]
	public class TestPageModelWithRuleSetForHandlers : PageModel {

		public IActionResult OnGetDefault() => Page();

		public IActionResult OnGetSpecified() {
			PageContext.SetRulesetForClientsideMessages("Foo");
			return Page();
		}

		public IActionResult OnGetMultiple() {
			PageContext.SetRulesetForClientsideMessages("Foo", "Bar");
			return Page();
		}

		public IActionResult OnGetDefaultAndSpecified() {
			PageContext.SetRulesetForClientsideMessages("Foo", "default");
			return Page();
		}
	}
}
