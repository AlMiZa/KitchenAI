# US-001-BE — Account creation — Backend

**Type:** User Story — Backend  
**Parent:** [US-001 Account creation](US-001-account-creation.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0 / Sprint 1

## Story

As a user I want to create an account so my inventory is persisted and secured server-side.

## Acceptance criteria

- Backend stores user with hashed password (bcrypt or Argon2).
- On successful registration a Household is automatically created and the user is assigned as owner.
- JWT bearer token is returned on successful registration and login.
- Passwordless magic-link flow sends a signed short-lived token to the user's email.

## Technical notes

- Use JWT bearer tokens for authentication.
- Hash passwords with bcrypt or Argon2 (or delegate to a secure auth provider).
- Passwordless magic link: generate a signed, short-lived token; email it; validate on callback.
- On registration, auto-create a `Household` and insert a `HouseholdMember` record with role `owner`.

## API endpoints

- `POST /api/auth/register` — Body: `{ email, password?, passwordless?: bool }` → Returns: user + token
- `POST /api/auth/login` — Body: `{ email, password }` → Returns: token
- `POST /api/auth/passwordless/request` — Body: `{ email }` → sends magic link
- `GET /api/auth/me` — Returns: current user profile

## Tests

- Backend unit test for registration handler: valid email → user and household created.
- Backend unit test: duplicate email → returns validation error.
- Backend unit test: passwordless request → token stored and email triggered.
