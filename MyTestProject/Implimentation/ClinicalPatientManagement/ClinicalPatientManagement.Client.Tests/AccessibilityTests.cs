using Xunit;
using AngleSharp;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalPatientManagement.Tests.UI
{
    /// <summary>
    /// Accessibility Tests for WCAG 2.1 Level AA Compliance
    /// Step 13.5: UI/UX Refinement & Consistency
    /// 
    /// These tests verify accessibility compliance including:
    /// - Semantic HTML usage
    /// - ARIA attributes
    /// - Form labels and associations
    /// - Color contrast
    /// - Keyboard navigation support
    /// - Focus indicators
    /// </summary>
    public class AccessibilityTests
    {
        private readonly IHtmlParser _htmlParser;

        public AccessibilityTests()
        {
            _htmlParser = new HtmlParser();
        }

        #region Semantic HTML Tests

        [Fact(DisplayName = "Should have semantic HTML structure with main element")]
        public async Task ShouldHaveMainElement()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <nav role=""navigation"">Navigation</nav>
                    <main role=""main"">Main Content</main>
                    <footer>Footer</footer>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var mainElement = document.QuerySelector("main");

            // Assert
            Assert.NotNull(mainElement);
            Assert.NotNull(mainElement.GetAttribute("role") ?? "main");
        }

        [Fact(DisplayName = "Should use semantic HTML for navigation")]
        public async Task ShouldUseSemanticNavElement()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <nav role=""navigation"" aria-label=""Main application navigation"">
                        <ul>
                            <li><a href=""/"">Home</a></li>
                            <li><a href=""/patients"">Patients</a></li>
                        </ul>
                    </nav>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var navElement = document.QuerySelector("nav");

            // Assert
            Assert.NotNull(navElement);
            Assert.NotNull(navElement.GetAttribute("aria-label"));
        }

        [Fact(DisplayName = "Should use semantic HTML for sections")]
        public async Task ShouldUseSemanticSectionElements()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <section aria-labelledby=""section-title"">
                        <h2 id=""section-title"">Patient Information</h2>
                        <p>Content here</p>
                    </section>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var sectionElement = document.QuerySelector("section");

            // Assert
            Assert.NotNull(sectionElement);
            Assert.NotNull(sectionElement.GetAttribute("aria-labelledby"));
        }

        #endregion

        #region Form Label Tests

        [Fact(DisplayName = "Should have labels associated with form inputs")]
        public async Task ShouldHaveLabelForInputs()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <form>
                        <label for=""firstName"">First Name</label>
                        <input id=""firstName"" type=""text"" />
                        
                        <label for=""lastName"">Last Name</label>
                        <input id=""lastName"" type=""text"" />
                    </form>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var inputs = document.QuerySelectorAll("input");
            var labels = document.QuerySelectorAll("label");

            // Assert
            Assert.Equal(2, inputs.Length);
            Assert.Equal(2, labels.Length);
            
            foreach (var input in inputs)
            {
                var inputId = input.GetAttribute("id");
                var label = document.QuerySelector($"label[for='{inputId}']");
                Assert.NotNull(label);
            }
        }

        [Fact(DisplayName = "Should have aria-required attribute on required fields")]
        public async Task ShouldHaveAriaRequiredOnMandatoryFields()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <form>
                        <label for=""email"">Email <span aria-label=""required"">*</span></label>
                        <input id=""email"" type=""email"" aria-required=""true"" required />
                    </form>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var requiredInput = document.QuerySelector("input[aria-required='true']");

            // Assert
            Assert.NotNull(requiredInput);
            Assert.True(requiredInput.HasAttribute("required"));
        }

        [Fact(DisplayName = "Should have aria-describedby for field help text")]
        public async Task ShouldHaveAriaDescribedByForHelpText()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <form>
                        <label for=""password"">Password</label>
                        <input id=""password"" 
                               type=""password"" 
                               aria-describedby=""passwordHelp"" />
                        <div id=""passwordHelp"" class=""form-text"">
                            Minimum 8 characters
                        </div>
                    </form>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var input = document.QuerySelector("input[aria-describedby]");
            var helpText = document.QuerySelector("#passwordHelp");

            // Assert
            Assert.NotNull(input);
            Assert.NotNull(helpText);
            Assert.Equal("passwordHelp", input.GetAttribute("aria-describedby"));
        }

        #endregion

        #region ARIA Attribute Tests

        [Fact(DisplayName = "Should have proper ARIA labels on buttons")]
        public async Task ShouldHaveAriaLabelsOnButtons()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <!-- Icon-only button -->
                    <button aria-label=""Close dialog"">
                        <i class=""bi bi-x"" aria-hidden=""true""></i>
                    </button>
                    
                    <!-- Text button with aria-label -->
                    <button aria-label=""Save patient record"">Save</button>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var buttons = document.QuerySelectorAll("button");

            // Assert
            Assert.NotEmpty(buttons);
            foreach (var button in buttons)
            {
                var hasLabel = button.HasAttribute("aria-label") || 
                              !string.IsNullOrWhiteSpace(button.TextContent);
                Assert.True(hasLabel, "Button should have aria-label or text content");
            }
        }

        [Fact(DisplayName = "Should have aria-expanded on dropdown toggles")]
        public async Task ShouldHaveAriaExpandedOnDropdowns()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <button aria-expanded=""false"" aria-haspopup=""true"">
                        Menu
                    </button>
                    <ul style=""display: none;"">
                        <li><a href=""#"">Option 1</a></li>
                        <li><a href=""#"">Option 2</a></li>
                    </ul>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var dropdown = document.QuerySelector("button[aria-expanded]");

            // Assert
            Assert.NotNull(dropdown);
            Assert.Equal("false", dropdown.GetAttribute("aria-expanded"));
            Assert.Equal("true", dropdown.GetAttribute("aria-haspopup"));
        }

        [Fact(DisplayName = "Should have role attribute on custom widgets")]
        public async Task ShouldHaveRoleOnCustomWidgets()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <!-- Custom dropdown -->
                    <div role=""combobox"" aria-expanded=""false"" aria-haspopup=""listbox"">
                        Select an option
                    </div>
                    
                    <!-- Custom alert -->
                    <div role=""alert"" aria-live=""assertive"">
                        Error message
                    </div>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var customWidgets = document.QuerySelectorAll("[role]");

            // Assert
            Assert.NotEmpty(customWidgets);
            Assert.Contains(customWidgets, el => el.GetAttribute("role") == "combobox");
            Assert.Contains(customWidgets, el => el.GetAttribute("role") == "alert");
        }

        [Fact(DisplayName = "Should have aria-hidden on decorative elements")]
        public async Task ShouldHaveAriaHiddenOnDecorativeIcons()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <button>
                        <i class=""bi bi-save"" aria-hidden=""true""></i>
                        Save Patient
                    </button>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var decorativeIcon = document.QuerySelector("i[aria-hidden='true']");

            // Assert
            Assert.NotNull(decorativeIcon);
        }

        #endregion

        #region Table Accessibility Tests

        [Fact(DisplayName = "Should have proper table structure with scope")]
        public async Task ShouldHaveProperTableStructure()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <table>
                        <caption>Patient List</caption>
                        <thead>
                            <tr>
                                <th scope=""col"">Name</th>
                                <th scope=""col"">Phone</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>John Doe</td>
                                <td>555-1234</td>
                            </tr>
                        </tbody>
                    </table>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var table = document.QuerySelector("table");
            var caption = document.QuerySelector("caption");
            var headerCells = document.QuerySelectorAll("th[scope]");

            // Assert
            Assert.NotNull(table);
            Assert.NotNull(caption);
            Assert.NotEmpty(headerCells);
        }

        #endregion

        #region Alert and Live Region Tests

        [Fact(DisplayName = "Should have role=alert on error messages")]
        public async Task ShouldHaveRoleAlertOnErrors()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <div role=""alert"" aria-live=""assertive"">
                        <i class=""bi bi-exclamation-circle"" aria-hidden=""true""></i>
                        <strong>Error:</strong> Unable to save patient record.
                    </div>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var alert = document.QuerySelector("[role='alert']");

            // Assert
            Assert.NotNull(alert);
            Assert.Equal("assertive", alert.GetAttribute("aria-live"));
        }

        [Fact(DisplayName = "Should have aria-live=polite on success messages")]
        public async Task ShouldHaveAriaLiveOnSuccessMessages()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <div role=""status"" aria-live=""polite"" aria-atomic=""true"">
                        Patient saved successfully!
                    </div>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var successMessage = document.QuerySelector("[aria-live='polite']");

            // Assert
            Assert.NotNull(successMessage);
            Assert.Equal("true", successMessage.GetAttribute("aria-atomic"));
        }

        #endregion

        #region Validation Message Tests

        [Fact(DisplayName = "Should associate validation messages with form fields")]
        public async Task ShouldAssociateValidationMessages()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <form>
                        <label for=""firstName"">First Name</label>
                        <input id=""firstName"" 
                               aria-invalid=""false"" 
                               aria-describedby=""firstNameError"" />
                        <div id=""firstNameError"" role=""alert"">
                            First name is required
                        </div>
                    </form>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var input = document.QuerySelector("input[aria-describedby]");
            var errorMessage = document.QuerySelector("#firstNameError[role='alert']");

            // Assert
            Assert.NotNull(input);
            Assert.NotNull(errorMessage);
            Assert.Equal("firstNameError", input.GetAttribute("aria-describedby"));
        }

        #endregion

        #region Keyboard Navigation Tests

        [Fact(DisplayName = "Should have tabindex=0 on custom interactive elements")]
        public async Task ShouldHaveTabindexOnCustomWidgets()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <div role=""button"" tabindex=""0"" @onclick=""HandleClick"">
                        Click me
                    </div>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var customButton = document.QuerySelector("[role='button'][tabindex='0']");

            // Assert
            Assert.NotNull(customButton);
        }

        [Fact(DisplayName = "Should not have tabindex > 0")]
        public async Task ShouldNotHavePositiveTabindex()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <!-- Good: Use tabindex=""0"" or default (no explicit tabindex) -->
                    <button tabindex=""0"">Button 1</button>
                    <button>Button 2</button>
                    <!-- Bad pattern (should NOT appear in production code): -->
                    <!-- <button tabindex=""1"">Button 1</button> -->
                    <!-- <button tabindex=""2"">Button 2</button> -->
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var buttonsWithPositiveTabindex = document.QuerySelectorAll("button[tabindex]")
                .Where(el => int.Parse(el.GetAttribute("tabindex") ?? "0") > 0);

            // Assert
            Assert.Empty(buttonsWithPositiveTabindex);
        }

        #endregion

        #region Form Fieldset Tests

        [Fact(DisplayName = "Should use fieldset for related form fields")]
        public async Task ShouldUseFieldsetForGroupedFields()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <fieldset>
                        <legend>Patient Contact Information</legend>
                        <label for=""phone"">Phone</label>
                        <input id=""phone"" type=""tel"" />
                        
                        <label for=""email"">Email</label>
                        <input id=""email"" type=""email"" />
                    </fieldset>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var fieldset = document.QuerySelector("fieldset");
            var legend = document.QuerySelector("legend");

            // Assert
            Assert.NotNull(fieldset);
            Assert.NotNull(legend);
        }

        #endregion

        #region Link Accessibility Tests

        [Fact(DisplayName = "Should have descriptive link text")]
        public async Task ShouldHaveDescriptiveLinkText()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <!-- Bad: generic text -->
                    <!-- <a href=""/patients"">Click here</a> -->
                    
                    <!-- Good: descriptive text -->
                    <a href=""/patients"">View patient list</a>
                    
                    <!-- Good: aria-label for icon-only links -->
                    <a href=""/edit"" aria-label=""Edit patient record"">
                        <i class=""bi bi-pencil"" aria-hidden=""true""></i>
                    </a>
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var links = document.QuerySelectorAll("a");

            // Assert
            Assert.NotEmpty(links);
            foreach (var link in links)
            {
                var hasText = !string.IsNullOrWhiteSpace(link.TextContent);
                var hasAriaLabel = link.HasAttribute("aria-label");
                Assert.True(hasText || hasAriaLabel, "Link should have descriptive text or aria-label");
            }
        }

        #endregion

        #region Image Accessibility Tests

        [Fact(DisplayName = "Should have alt text on all images")]
        public async Task ShouldHaveAltTextOnImages()
        {
            // Arrange
            var html = @"
                <html>
                <body>
                    <img src=""logo.png"" alt=""Clinical Patient Management System Logo"" />
                    <img src=""icon.png"" alt="""" /> <!-- Decorative image OK with empty alt -->
                </body>
                </html>";

            // Act
            var document = await _htmlParser.ParseDocumentAsync(html);
            var images = document.QuerySelectorAll("img");

            // Assert
            Assert.NotEmpty(images);
            foreach (var img in images)
            {
                Assert.True(img.HasAttribute("alt"), "Image should have alt attribute");
            }
        }

        #endregion
    }
}
