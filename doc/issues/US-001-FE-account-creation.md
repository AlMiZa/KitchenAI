# US-001-FE — Account creation — Frontend

**Type:** User Story — Frontend  
**Parent:** [US-001 Account creation](US-001-account-creation.md)  
**Priority:** Must-have (MVP)  
**Sprint:** Sprint 0 / Sprint 1

## Story

As a user I want a clear sign-up and sign-in interface so I can create and access my account.

## Acceptance criteria

- Registration form validates email format and minimum password strength before submission.
- After successful registration the user is redirected to the Dashboard.
- Sign-in form shows an error message on invalid credentials.
- Passwordless option displays a "Check your email" confirmation after submitting the magic-link request.

## Technical notes

- Use React Hook Form or controlled components for form state.
- Show inline validation errors on blur/submit.
- Call `POST /api/auth/register` and `POST /api/auth/login`; store JWT in a secure location (e.g., `httpOnly` cookie or memory with refresh-token pattern).
- Redirect to `/dashboard` on successful authentication using React Router.

## UI

- Screen: **Sign up / Sign in**
  - Tab or toggle between "Sign up" and "Sign in" forms.
  - Example copy: "Create your KitchenAI account — use your email to save and sync your fridge."
  - Passwordless option: "Send me a magic link" button below the password field.

## Tests

- Frontend unit test: register form shows validation error for empty email.
- Frontend unit test: register form shows validation error for invalid email format.
- Frontend unit test: successful registration dispatches navigation to `/dashboard`.
