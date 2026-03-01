# Prompts Log

This file logs all prompts used during the KitchenAI project development with GitHub Copilot.

## Prompts

| # | Prompt | Description |
|---|--------|-------------|
| 1 | Create PROMPTS.MD file to log all my prompts. | Create a markdown file to track all GitHub Copilot prompts used in this project. |
| 2 | Add a new feature specification document to the repository at doc/feature-specifications.md. | Create the KitchenAI mid-level feature specification document covering executive summary, MVP scope, personas, data model, REST API outline, user stories with acceptance criteria, developer notes (React + ASP.NET Core), AI recipe generation design, GDPR/privacy, localization, and roadmap. |
| 3 | Split specification on issues | Split the monolithic feature specification document into separate issue files (one per user story / technical task) under doc/issues/. |
| 4 | Split the issues into frontend and backend tasks where it makes sense | For each issue that contains both frontend and backend work, create `-BE` and `-FE` sub-issue files. Update parent issue files to reference the sub-issues. Update `doc/issues/README.md` to show the full hierarchy. TECH-002 (data model) is backend-only and is not split. |
| 5 | /create-issue Create all missed issues based on *.md files from doc/issues folder. | Create a GitHub Actions workflow (`.github/workflows/create-issues.yml`) that reads each `doc/issues/*.md` file, extracts the issue title from the H1 heading, and creates a GitHub issue for any file that does not already have a corresponding issue. |
| 6 | TECH-001-FE — Project setup & architecture — Frontend | Scaffold the React SPA under `/frontend` using Vite 6 + React 19 + TypeScript. Install and configure React Router v7, Tailwind CSS v4, TanStack Query v5, and react-i18next. Set up Jest 30 + React Testing Library for unit tests. Create placeholder pages (Dashboard, Inventory, Recipes, ShoppingList, Settings, Admin/ApiKeys, Admin/Metrics), MainLayout and AdminLayout components, EN/PL translation files, and a GitHub Actions CI workflow (`frontend-ci.yml`) running lint → test → build on every PR. |
| 7 | update PROMPTS.MD | Update PROMPTS.MD to log the prompts used for the TECH-001-FE frontend scaffold implementation. |
