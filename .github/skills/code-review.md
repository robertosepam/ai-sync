---
applyTo: "Backend/**"
---

---
name: code-review
description: "Baseline to review C# code in this project"
---
When reviewing C# code in this project, consider the following aspects:

1.- **Naming Conventions**: Ensure that variable, method, class, and namespace names are descriptive and follow C# naming conventions (PascalCase for classes and methods, camelCase for variables).
2.- **Code Structure**: Check that the code is organized into appropriate classes and methods, with a clear separation of concerns. Each method should have a single responsibility.
3.- **Error Handling**: Verify that exceptions are properly handled and that the code does not swallow exceptions without logging or rethrowing them.
4.- **Performance**: Look for any potential performance issues, such as unnecessary object creation, inefficient algorithms, or blocking calls in asynchronous code.
5.- **Security**: Check for any potential security vulnerabilities, such as SQL injection, cross-site scripting (XSS), or improper handling of sensitive data.
6.- **Code Readability**: Ensure that the code is easy to read and understand, with appropriate comments and documentation where necessary. Avoid overly complex logic that could be simplified.
7.- **Code Style**: Ensure all return statements have an empty line before them, and that there is an empty line after the last statement in a method. This improves readability and maintains a consistent code style across the project.

Provide a list with the issues found, categorized by severity (e.g., Critical, Major, Minor) and include specific line numbers and code snippets where applicable. Additionally, suggest improvements or best practices to address each issue.