# US-001 — Account creation

**Type:** User Story  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0 / Sprint 1

> This story is split into frontend and backend sub-issues:
> - **Backend**: [US-001-BE — Account creation — Backend](US-001-BE-account-creation.md)
> - **Frontend**: [US-001-FE — Account creation — Frontend](US-001-FE-account-creation.md)

## Story

As a user I want to create an account to persist my inventory across devices.

## Acceptance criteria

- User can register with email + password or request a passwordless magic link.
- After registration user is redirected to Dashboard with an empty household created.
- Backend stores user with hashed password.

## Technical notes

- Use JWT bearer tokens for authentication.
- Hash passwords with bcrypt or Argon2 (or delegate to a secure auth provider).
- Passwordless magic link implemented via signed short-lived token emailed to user.
- On registration automatically create a Household and assign the user as owner.

## API endpoints

- `POST /api/auth/register` — Body: `{ email, password?, passwordless?: bool }` → Returns: user + token
- `POST /api/auth/login` — Body: `{ email, password }` → Returns: token
- `POST /api/auth/passwordless/request` — Body: `{ email }` → sends magic link
- `GET /api/auth/me` — Returns: current user profile

## Tests

- Backend unit test for registration handler: valid email → user and household created.
- Frontend unit test: register form validation.

## UI

- Screen: **Sign up / Sign in**
- Example copy: "Create your KitchenAI account — use your email to save and sync your fridge."
