---
applyTo: "Frontend/src/**"
---

# UI Style — Frontend Components
- All views must follow a responsive design, ensuring usability across various devices and screen sizes.
- Use a consistent color scheme and typography throughout the application, adhering to the established design system.
- Components should be modular and reusable, following the principles of component-based architecture.
- Ensure accessibility by following WCAG guidelines, including proper use of ARIA attributes and keyboard navigation support.

# TypeScript
- Use strict typing and interfaces to define the shape of data and component props.
- Follow the Airbnb TypeScript style guide for code formatting and best practices.
- Use async/await for asynchronous operations and handle errors gracefully.
- Organize code into modules and namespaces to maintain a clear structure and separation of concerns.
- Use ESLint with a consistent configuration to enforce code quality and style across the codebase.

# Angular Components
- Use Angular's built-in features such as services for data handling and directives for DOM manipulation.
- Follow Angular's style guide for naming conventions, file structure, and coding practices.
- Ensure components are properly encapsulated, with clear inputs and outputs, and avoid tight coupling between components.
- For Angular ≥ 19, prefer input(), output(), viewChild() functions over decorators.

# CSS Files
- Use SCSS with component-level encapsulation.
- Avoid using in-style styles; instead, define styles in separate SCSS files and import them into the component.
- Follow a consistent naming convention for CSS classes, such as BEM (Block Element Modifier) or a similar methodology.
- Use variables and mixins to maintain a consistent design system and reduce code duplication in stylesheets.