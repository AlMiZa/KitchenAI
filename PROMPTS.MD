# Prompts Log

This file logs all prompts used during the KitchenAI project development with GitHub Copilot.

## Prompts

| # | Prompt | Description |
|---|--------|-------------|
| 1 | Create PROMPTS.MD file to log all my prompts. | Create a markdown file to track all GitHub Copilot prompts used in this project. |
| 2 | Add a new feature specification document to the repository at doc/feature-specifications.md. | Create the KitchenAI mid-level feature specification document covering executive summary, MVP scope, personas, data model, REST API outline, user stories with acceptance criteria, developer notes (React + ASP.NET Core), AI recipe generation design, GDPR/privacy, localization, and roadmap. |
| 3 | Split specification on issues | Split the monolithic feature specification document into separate issue files (one per user story / technical task) under doc/issues/. |
| 4 | Split the issues into frontend and backend tasks where it makes sense | For each issue that contains both frontend and backend work, create `-BE` and `-FE` sub-issue files. Update parent issue files to reference the sub-issues. Update `doc/issues/README.md` to show the full hierarchy. TECH-002 (data model) is backend-only and is not split. |
