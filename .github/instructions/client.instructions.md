---
applyTo: "kc.lms.client/**"
---

# Client (React + TypeScript) Rules

- Functional components with hooks only; no class components.
- TypeScript strict mode — no `any`; prefer explicit prop interfaces.
- Components live in `src/`; one component per file, named exports preferred.
- Use `fetch` against the API (Vite dev proxy targets KC.LMS.Server).
- Run `npm run lint` (oxlint) and `npm run build` (tsc + vite) to validate changes.
- Keep styling consistent with existing CSS approach; avoid adding UI libraries without asking.
