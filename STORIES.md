# LearnCSharp — Product Stories & Delivery Plan

An interactive **static web app** that teaches C# from first principles to advanced — for
**GCSE students (Y9–11), A-Level students (Y12–13), and working professionals** alike.
62 illustrated lessons, ~110 diagrams, 620 quiz questions and printable worksheets, on three
guided pathways through one shared body of content. Hosted on Azure at no cost.

**Status:** in delivery — **live at
[red-dune-0f7372e10.7.azurestaticapps.net](https://red-dune-0f7372e10.7.azurestaticapps.net)** ·
**Last updated:** 17 August 2026

---

## 0. Delivery status — at a glance

Legend: ✅ done · 🟡 partly done · ⬜ not started. Each story below carries the same tag.

| Area | State |
|---|---|
| **Deployed site** | ✅ live on Azure Static Web Apps (Free), CI-gated deploys on every merge to `main` |
| **Lesson code** | ✅ 57 / 62 compile and run (Module 6's five remain unwritten) |
| **Theory notes** | ✅ **57 / 57 written lessons have full notes** — Modules 1–5 and 7 complete |
| **Verified output pipeline** | ✅ capture → JSON → site, regenerated every build; a failing lesson fails the build |
| **Summaries & objectives** | ✅ all 57 lessons self-describing on cards, syllabus and lesson pages |
| **Site features** | ✅ home, accordion syllabus with deep links, lesson pages, prev/next, 6-theme picker, mobile pass |
| **Diagrams** | 🟡 13 of ~110 (the most spatial concepts), themed SVG |
| **Quizzes** | 🟡 the player works; **only 10 questions exist (lesson 1.4)** of the 620 planned |
| **Progress** | 🟡 localStorage store + per-module bars; no dashboard, export UI, streaks or revision |
| **Module 6 (Production C#)** | ⬜ the five lessons — the only C# left to write |
| **Worksheets, reference, search, pathways, module tests, topic exams** | ⬜ not started |

**The biggest gap between promise and product is the question bank** (E4/US-406): the product
story is "never more than one screen of theory before something to do", and 56 of 57 lessons
currently end without a quiz.

---

## 1. The story

> **Alex, Year 10.** Alex has just finished GCSE Computer Science (AQA). They can write
> pseudocode, they know what a `FOR` loop is, and they have heard that "real programmers use C#".
> They open a C# tutorial, hit `static void Main(string[] args)` on line one, and close the tab.

The app exists to make the next 20 weeks of Alex's life work like this instead:

1. **Monday evening.** Alex opens the site on a laptop. The home page shows a progress ring:
   *Module 1, 4 of 11 lessons done.* One button: **Continue → 1.5 Selection**.
2. **The lesson** opens as a single scrollable page. It starts from what Alex already knows —
   the AQA `IF … ENDIF` block — and shows the C# beside it. Code is syntax-highlighted and
   annotated. Where output matters, the real output is shown underneath, captured from a program
   that genuinely ran.
3. **Halfway down**, a *Check yourself* card interrupts: "What does `17 / 5` print?" Alex picks
   `3.4`, gets it wrong, and the card explains integer division on the spot — the correction
   lands at the exact moment the misconception exists.
4. **At the end**, a 10-question quiz. Alex scores 7/10. The two wrong answers link straight back
   to the paragraphs that cover them. The lesson is marked *needs review*, not *done*.
5. **Saturday.** Alex prints the Module 1 worksheet, writes code in Visual Studio Code, and
   checks the answers against the model solutions.
6. **Six weeks later** the dashboard shows Module 1 complete, a 12-day streak, and a weak-topic
   list: *arrays, string formatting*. Alex taps **Revise weak topics** and gets a mixed quiz
   built only from those.
7. **Twenty weeks later** Alex is reading about Dijkstra's algorithm and understands it, because
   every step from `Console.WriteLine` to here was one small increment with a checkpoint.

**The product promise:** *never more than one screen of theory before something to do.*

---

## 2. Who it is for

| Persona | Level | Needs | Success looks like |
|---|---|---|---|
| **Alex** — Year 10, GCSE | Beginner | Small steps, plain English, exam-spec links, constant reassurance | Finishes a module without giving up; writes a working console app unaided |
| **Priya** — Year 12, A-Level | Intermediate | Depth on OOP, data structures and complexity; NEA project help; exam technique | Designs a class model unaided and analyses an algorithm's Big-O |
| **Marcus** — professional, 8 yrs Java | Advanced | Zero hand-holding; C# specifics; "what is different from Java"; async, LINQ, DI | Productive in C# inside a week; uses the site as a reference thereafter |
| **Sam** — teacher/tutor | — | Homework to set; specification coverage; worksheets and answers | Assigns "Module 2 + worksheet" and can see it was completed |

### 2.1 One body of content, three pathways

The same 62 lessons, entered three different ways. Nobody is shown a stripped-down version —
the pathway changes the **route, framing and emphasis**, never the ceiling.

| | 🟢 **GCSE (Y9–11)** | 🔵 **A-Level (Y12–13)** | 🟣 **Professional** |
|---|---|---|---|
| **Entry point** | 1.1 Hello World | 2.1 What is OOP? (with a Module 1 refresher offered) | 2.1, or straight to the "C# for experienced developers" fast track |
| **Route** | Modules 1 → 2 → 3, then 5 | Modules 1 → 5 in full, plus 6 | Fast track, then 3 → 4 → 6 by need |
| **Pace shown** | ~24 weeks | ~16 weeks | ~2 weeks |
| **Framing** | "from your GCSE pseudocode…" | "…and how it is examined" | "…and how it differs from Java/Python" |
| **Extras surfaced** | AQA GCSE spec links, exam tips | A-Level spec links, NEA project guidance, complexity proofs | Idioms, pitfalls, performance notes, "don't do this in production" |
| **Depth toggles** | collapsed by default | expanded | expanded |
| **Tone** | encouraging, jargon defined on first use | precise, exam-accurate | terse, assumes fluency in another language |

---

## 3. Decisions (locked)

| Decision | Choice | Why |
|---|---|---|
| **Hosting** | **Azure Static Web Apps — Free tier** | £0 forever for this workload; free SSL, custom domain, global CDN, GitHub Actions built in. See §3.2 |
| **Stack** | **React + TypeScript (Vite)** | Chosen for maintainability — the person who owns this repo knows React. Astro would ship less JS, but a framework nobody on the team knows is the bigger risk. See §3.3 |
| **Code execution** | **None in-browser.** Snippets are highlighted and read-only | Smallest, fastest, most reliable build. Students write real code in their own IDE, which is the skill we want anyway |
| **Output shown** | Pre-captured from the existing .NET console project | Output is *real*, never hand-written and wrong |
| **Progress storage** | **`localStorage` only**, with JSON export/import | No accounts, no backend, no personal data leaves the device |
| **Content source** | The existing `docs/*.md` and `src/**/L*.cs` | ~2,300 lines of notes and ~8,700 lines of verified lesson code already written |

### 3.1 The console app stays — as the correctness harness

The existing .NET project is **not** thrown away. It becomes the guarantee that every snippet on
the site is true:

```
src/LearnCSharp.Lessons/        57 lessons, all compiling, all running
        |
        |  build step: dotnet run -- capture  -> public/data/*.json
        v
web/src/content/                Markdown + captured real output
        |
        |  vite build
        v
dist/                           the static site
```

If a snippet stops compiling, CI fails. **No lesson can ever show output the code does not
actually produce.**

### 3.3 What React costs us, and the plan for it

React was chosen over Astro deliberately, but it is not free. Recording the trade-off honestly
so nobody is surprised later:

| | Astro | React SPA (what we built) |
|---|---|---|
| JS on a content page | ~0 KB | 75 KB gzipped, +102 KB when a lesson opens |
| Markdown rendering | build time | **client side**, via react-markdown |
| Routing | static files per page | client side, HashRouter |
| Familiarity here | low | **high** |

**Current measured bundle** (`npm run build`):

```
index      234 KB  ->  75 KB gzipped     home + syllabus
Lesson     330 KB  -> 102 KB gzipped     lazy, only when a lesson is opened
```

US-803 targets under 50 KB of JS per page, which this does **not** yet meet. Two fixes are
already identified, neither of which changes the framework:

1. **Pre-render Markdown to HTML at build time** and drop `react-markdown` from the client
   entirely. This is the big one — it removes most of the 102 KB lesson chunk.
2. **Pre-highlight code at build time** with Shiki, removing `highlight.js` too.

Both are build-step changes behind the same components, so they can land any time. Until then
the site is fast enough to develop against and the architecture is right.

### 3.2 Azure hosting — and how it stays free

**Azure Static Web Apps (Free tier)** is the target. It is designed for exactly this: a static
site built by GitHub Actions and served from a global CDN.

| Free tier gives us | Limit | What we actually need | Headroom |
|---|---|---|---|
| Bandwidth | 100 GB / month | ~2 MB per student session × 2,000 sessions ≈ 4 GB | 25× |
| App size | 250 MB per deployment | 56 lessons of HTML/CSS/JS ≈ 15 MB | 16× |
| Custom domains | 2, with free managed SSL | 1 | ✅ |
| Staging environments | 3 | 1 preview per PR | ✅ |
| Global CDN + HTTPS | included | required | ✅ |
| Build minutes | via GitHub Actions free tier | ~3 min per deploy | ✅ |

**Rules that keep the bill at zero:**

1. **No Azure Functions.** The Free tier bundles a managed Functions API — the moment we use it
   we are on the path to a paid plan. Everything stays client-side (§3 decision on `localStorage`).
2. **No Application Insights**, no Azure Storage account, no Key Vault, no database. A static
   site needs none of them.
3. **Stay on Free, never Standard.** Standard is ~£7/month and buys SLA, more domains and larger
   apps — none of which we need.
4. **Optimise payload anyway** (US-803): smaller pages mean the 100 GB is unreachable even if the
   site is a hit.
5. **Set a £0 budget alert** on the subscription as a tripwire, so any accidental paid resource is
   noticed the same day.

> **Fallback:** if Azure's free tier ever changes, the build output is a plain `dist/` folder.
> Moving to GitHub Pages, Netlify or Cloudflare Pages is a one-file change to the workflow. We
> are deliberately not locked in.

---

## 4. Scope

**In scope (v1)**

- 62 lessons across 7 modules, covering all 31 core C# topics (§8)
- Three guided pathways — GCSE, A-Level, professional — over one shared body of content
- ~110 diagrams — every structural or temporal concept is drawn, not just described
- One end-of-lesson quiz per lesson; one end-of-module test per module
- Inline "check yourself" questions inside lessons
- Printable worksheets, one per module, with model answers
- Progress tracking, streaks, weak-topic revision
- Search, syllabus, glossary, GCSE-pseudocode → C# reference, topic-coverage map
- Dark mode, mobile, keyboard navigation, screen-reader support

**Out of scope (v1)** — recorded as future work in §10

- Running or compiling C# in the browser
- Accounts, cloud sync, tutor dashboards, class management
- Video, audio, certificates
- Anything requiring a server

---

## 5. Site map

```
/                          home - choose your pathway, or continue where you left off
/pathway/gcse              GCSE route: ordered lessons, spec coverage, exam tips
/pathway/a-level           A-Level route: fuller depth, NEA guidance
/pathway/professional      fast track for experienced developers
/fast-track                "C# in 2 weeks if you already program" - 12 lessons
/syllabus                  all 6 modules, 51 lessons, progress per lesson
/module/1                  module overview, lesson list, worksheet, module test
/module/1/1.4              a lesson page
/module/1/1.4/quiz         the lesson quiz
/module/1/test             end-of-module test
/module/1/exam             topic exam - pick Foundation / Standard / Challenge
/module/1/exam/results     marks, grade band, per-topic breakdown, full review
/module/1/worksheet        printable worksheet
/module/1/worksheet/answers   model answers
/progress                  dashboard: completion, scores, streak, weak topics
/revise                    mixed quiz generated from weak topics
/reference/pseudocode      AQA pseudocode -> C# translation table
/reference/from-java       C# for Java developers - the differences that bite
/reference/from-python     C# for Python developers - the differences that bite
/reference/specs           GCSE and A-Level specification coverage
/reference/glossary        every term, defined, linked to its lesson
/reference/cheatsheets     one printable card per module
/reference/coverage        the 31 core C# topics, each mapped to its lesson
/search                    full-text search across all lessons
/about                     how to use the site, how to install .NET
```

---

## 6. Content model

Each lesson is one Markdown file with typed frontmatter. Astro's content collections validate
this at build time, so a malformed lesson breaks the build rather than the site.

```yaml
---
id: "1.4"
module: 1
title: "Operators and expressions"
summary: "Arithmetic, comparison and logical operators - including the integer-division trap."
objectives:
  - "Use arithmetic, comparison and logical operators correctly"
  - "Explain why 17 / 5 is 3 in C#"
  - "Use % to solve wrap-around and time-splitting problems"
prerequisites: ["1.2", "1.3"]
estimatedMinutes: 25
difficulty: beginner          # beginner | intermediate | advanced
audiences: [gcse, alevel, professional]     # who this lesson appears for
pathwayOrder:
  gcse: 4                     # position within each pathway
  alevel: 4
  professional: skip          # 'skip' = not on the fast track, still reachable
gcseLinks: ["3.1.2 Arithmetic operations", "3.1.3 Boolean operations"]
aLevelLinks: ["4.1.1 Data types", "4.1.2 Programming concepts"]
sourceFile: "src/LearnCSharp.Lessons/Lessons/M1_Foundations/L04_Operators.cs"
---
```

### 6.1 Code and output blocks

````markdown
```csharp
int x = 17, y = 5;
Console.WriteLine(x / y);
```

<Output lesson="1.4" marker="integer-division" />
````

`<Output>` pulls from the JSON captured by the console app. Output is never typed by hand.

### 6.2 Question schema

```yaml
- id: "1.4-q3"
  type: predict-output          # see types below
  difficulty: 2                 # 1-3
  topic: "integer division"
  stem: "What does this print?"
  code: |
    int total = 23, count = 4;
    Console.WriteLine(total / count);
  options: ["5.75", "5", "6", "Compile error"]
  answer: 1
  explanation: "Both operands are int, so C# does integer division and discards the .75."
  reviewLink: "1.4#arithmetic"
```

`difficulty` does double duty: quizzes mix all three levels, while **topic exams (US-407)
build their easy/medium/hard sets from it and award it as marks** — a difficulty-3 question
is worth 3 marks. One bank, no duplicate authoring.

### 6.3 Diagrams

Diagrams are **content, not decoration**. Half the topics in this course are spatial — memory,
references, trees, queues, threads — and a paragraph describing a binary search tree will never
teach as well as a picture of one.

**Rule:** if a concept involves *structure, flow or time*, it gets a diagram.

```markdown
<Diagram src="stack-vs-heap" caption="Where each kind of variable lives"
         alt="A stack frame holding an int and a reference arrow pointing to an object on the heap" />
```

| Approach | Used for | Client JS |
|---|---|---|
| **Inline SVG** (authored) | memory layouts, node diagrams, anything bespoke | none |
| **Mermaid, rendered at build time** | flowcharts, class diagrams, sequence diagrams | none |
| **Stepper islands** | algorithms that unfold: sorting passes, BFS, recursion | ~4KB, lazy-loaded |

All three must: use the site's colour tokens, work in light **and** dark, carry a real `alt`
description, and remain legible at 320px.

### 6.4 Question types

| Type | Looks like | Best for |
|---|---|---|
| `multiple-choice` | one right answer of four | definitions, concepts |
| `multi-select` | "select all that apply" | "which of these are value types?" |
| `true-false` | two options | quick misconception checks |
| `predict-output` | code block → what prints? | the highest-value type; catches real misunderstanding |
| `spot-the-bug` | code with one fault, click the line | debugging skill |
| `fill-the-blank` | code with gaps, choose the token | syntax fluency |
| `ordering` | drag steps into order | algorithms — bubble sort passes, BFS steps |
| `matching` | term ↔ definition | vocabulary, Big-O ↔ algorithm |
| `short-answer` | free text, self-marked against a model answer | explanation practice, exam style |

---

## 7. Epics

| # | Epic | Goal |
|---|---|---|
| **E1** | Foundation & shell | A deployed, navigable site skeleton |
| **E2** | Lesson content pipeline | 51 lessons rendered from Markdown, with verified output |
| **E3** | Navigation & discovery | Nobody is ever lost or has to hunt |
| **E4** | Quizzes & assessment | Every lesson ends in a checkpoint |
| **E5** | Worksheets | Offline, written practice with model answers |
| **E6** | Progress & motivation | Visible progress; targeted revision |
| **E7** | Reference & revision | Fast lookup for a student mid-task |
| **E8** | Quality & accessibility | Fast, accessible, works on a phone and offline |
| **E9** | Authoring & CI | Adding a lesson is easy and cannot silently break |
| **E10** | Diagrams & visualisation | Every structural or temporal concept is *shown*, not just described |
| **E11** | Audience pathways | One site that suits a 14-year-old and a senior developer equally |

---

## E1 — Foundation & shell

### US-101 · Project skeleton *(Must, M)* · ✅ done
**As a** developer **I want** a typed, buildable web project **so that** content can be built
into a static site. *(Delivered with React + Vite per the §3 stack decision, not Astro.)*

- [x] `web/` created with Vite + React, TypeScript strict
- [x] `npm run dev`, `npm run build`, `npm run preview` all work
- [x] `npm run build` produces `dist/` with no server dependency
- [x] README documents setup

### US-102 · Site shell and layout *(Must, M)* · 🟡 partial — *header/footer/theme picker live; no sidebar, search box or progress ring*
**As a** student **I want** consistent header, sidebar and footer **so that** every page feels
like one product.

- [ ] Header: logo, search, module nav, progress ring, theme toggle
- [ ] Sidebar: current module's lessons, current one highlighted
- [ ] Footer: previous/next lesson, "edit this page" link
- [ ] Sidebar collapses to a drawer under 768px

### US-103 · Deploy pipeline to Azure *(Must, S)* · ✅ done — *deployed and gated; custom domain and budget alert still open*
**As a** developer **I want** a push to `main` to publish the site **so that** shipping is not a
manual chore.

- [ ] Azure Static Web App created on the **Free** tier, linked to the GitHub repo
- [ ] `azure-static-web-apps.yml` builds `web/` and deploys `dist/`
- [ ] Build fails ⇒ deploy does not happen
- [ ] Pull requests deploy to a staging environment and comment the preview URL
- [ ] **No API/Functions folder is configured** — this is what keeps the tier free
- [ ] Custom domain with managed SSL
- [ ] A £0 budget alert exists on the subscription

### US-104 · Design system *(Should, M)* · 🟡 partial — *tokens, palettes and type scale in; no callout components*
**As a** student **I want** readable typography and clear code styling **so that** long study
sessions do not hurt.

- [ ] Type scale, spacing scale, colour tokens as CSS custom properties
- [ ] Body text 16–18px, line length capped at ~70 characters
- [ ] Light and dark palettes, both meeting WCAG AA
- [ ] Callout components: Note, Careful, Exam tip, Your turn

---

## E2 — Lesson content pipeline

### US-201 · Content collection with schema *(Must, M)* · ⬜
**As an** author **I want** typed frontmatter **so that** a broken lesson fails the build, not
the student.

- [ ] Zod schema matching §6, enforced by Astro content collections
- [ ] Missing or malformed field ⇒ build error naming the file
- [ ] `prerequisites` must reference lessons that exist

### US-202 · Lesson page template *(Must, M)* · 🟡 partial — *title/summary/objectives/body/output/quiz render; no TOC, prereq chips or time estimate*
**As a** student **I want** every lesson laid out the same way **so that** I know where to look.

- [ ] Renders: title · estimated time · objectives · body · exercises · quiz call-to-action
- [ ] Auto-generated "on this page" table of contents
- [ ] Prerequisite chips linking back to earlier lessons
- [ ] "Mark as complete" button at the foot

### US-203 · Migrate existing notes *(Must, L)* · ✅ done — *single-sourced from docs/ via the content build step*
**As an** author **I want** the 13 written Markdown docs moved into the site **so that** nothing
is rewritten twice.

- [ ] `docs/module-1/*.md` (11) and `docs/module-2/2.1–2.2.md` migrated with frontmatter
- [ ] Relative links rewritten to site routes
- [ ] Existing tables, callouts and code blocks render correctly

### US-204 · Write the remaining 38 lessons *(Must, XL)* · 🟡 partial — *57/57 written-lesson notes done; Module 6 lessons (code + notes) remain*
**As a** student **I want** all six modules **so that** the course is complete.

- [ ] Module 2 (2.3–2.9), Module 3 (3.1–3.7), Module 4 (4.1–4.11), Module 5 (5.1–5.9),
      Module 6 (6.1–6.4)
- [ ] Each follows: idea → syntax → detail → common mistakes → exercises
- [ ] Each references its verified `.cs` lesson file
- [ ] Each includes the diagrams listed for it in §8.3

### US-205 · Syntax highlighting *(Must, S)* · 🟡 partial — *highlight.js client-side, C#-only; Shiki-at-build and copy buttons pending*
**As a** student **I want** C# coloured the way my IDE colours it **so that** the site and VS Code
look like the same language.

- [ ] Shiki at build time (zero client JS), C# grammar, light + dark themes
- [ ] Line highlighting and line numbers where useful
- [ ] Copy-to-clipboard button on every block
- [ ] Diff blocks for "wrong vs right" comparisons

### US-206 · Verified output blocks *(Must, M)* · ✅ done — *per-lesson JSON, section-addressable, regenerated every build*
**As a** student **I want** to see exactly what the code prints **so that** I can check my
understanding without running anything.

- [ ] `dotnet run -- all` captured to JSON at build time
- [ ] `<Output>` component renders the captured text in a terminal style
- [ ] A missing marker fails the build
- [ ] Long output collapses with "show all"

### US-207 · Inline check-yourself cards *(Should, M)* · ⬜
**As a** student **I want** a question in the middle of a lesson **so that** misconceptions are
caught as they form.

- [ ] `<CheckYourself>` component, 1–3 per lesson
- [ ] Answering reveals the explanation immediately
- [ ] Does **not** count towards quiz scores
- [ ] Fully usable by keyboard

---

## E3 — Navigation & discovery

### US-301 · Syllabus page *(Must, M)* · ✅ done — *accordion per module, progress bars and badges*
**As a** student **I want** the whole course on one page **so that** I can see where I am going.

- [ ] All 6 modules, 51 lessons, grouped, with estimated times
- [ ] Per-lesson state: not started · in progress · done · needs review
- [ ] Module completion bars
- [ ] Filter by difficulty; jump to first incomplete lesson

### US-302 · Continue where you left off *(Must, S)* · ✅ done
**As a** returning student **I want** one button that resumes **so that** I never hunt for my
place.

- [ ] Home shows the next incomplete lesson as the primary action
- [ ] First-time visitors see "Start at 1.1" instead

### US-303 · Previous / next navigation *(Must, S)* · 🟡 partial — *prev/next live; no keyboard shortcuts or module-test hand-off*
- [ ] Every lesson has prev/next with the real title
- [ ] Last lesson of a module points at the module test
- [ ] `←` / `→` keyboard shortcuts

### US-304 · Full-text search *(Should, M)* · ⬜
**As a** student **I want** to search everything **so that** I can find "yield" without knowing
which lesson it is in.

- [ ] Pagefind or Fuse.js index built at build time, no server
- [ ] `/` focuses the box; results show lesson, module and matching snippet
- [ ] Matches lesson text, code and glossary terms
- [ ] Usable entirely by keyboard

### US-305 · Module overview pages *(Must, S)* · 🟡 partial — *the syllabus accordion serves this; no separate pages*
- [ ] What the module covers, what you will be able to do, prerequisites
- [ ] Lesson list with progress; links to worksheet and module test

---

## E4 — Quizzes & assessment

### US-401 · Quiz data model and loader *(Must, M)* · 🟡 partial — *JSON per lesson with topic tags; no build-time validation*
- [ ] Questions authored as YAML/JSON per lesson, validated against §6.2
- [ ] Build fails on: no correct answer, duplicate IDs, missing explanation
- [ ] Questions carry a `topic` tag, used later for weak-topic revision

### US-402 · Quiz player *(Must, L)* · 🟡 partial — *one-at-a-time with instant explanations; no back-navigation or section links*
**As a** student **I want** to answer questions one at a time with instant feedback **so that** I
learn from mistakes while they are fresh.

- [ ] One question per screen, progress indicator
- [ ] Immediate right/wrong with the explanation
- [ ] Wrong answers link back to the exact section
- [ ] Cannot skip forward without answering; can go back to review
- [ ] Fully keyboard-operable, announced correctly by screen readers

### US-403 · Question type components *(Must, L)* · 🟡 partial — *multiple-choice only — 1 of 9 types*
- [ ] All nine types in §6.3 implemented
- [ ] Each has a keyboard path and a touch path
- [ ] `ordering` works with drag **and** with arrow-key move up/down
- [ ] `short-answer` reveals a model answer and asks the student to self-mark

### US-404 · Results screen *(Must, M)* · 🟡 partial — *score, misses, retry-all and the 80% rule; no per-topic breakdown*
- [ ] Score, time taken, per-topic breakdown
- [ ] List of missed questions with explanations
- [ ] "Retry wrong ones only" and "Retry all"
- [ ] ≥80% marks the lesson complete; below that marks it *needs review*

### US-405 · End-of-module tests *(Must, M)* · ⬜
**As a** student **I want** a longer test after each module **so that** I know I have retained
it, not just recognised it.

- [ ] 20–30 questions drawn from every lesson in the module
- [ ] Randomised order; no explanations until the end
- [ ] Pass mark 70%; result recorded per attempt
- [ ] Failing suggests the specific lessons to revisit

### US-406 · Question bank *(Must, XL)* · ⬜ — *10 of 620+ questions exist (lesson 1.4)*
- [ ] ≥10 questions per lesson → **510+**
- [ ] ≥25 per module test → **150+**
- [ ] Balanced across difficulty 1–3
- [ ] At least three `predict-output` questions per lesson, generated from verified code

### US-407 · Topic exams with marks and graded sets *(Should, L)* · ⬜
**As a** student **I want** a marked exam per topic at easy, medium or hard level **so that** I
can test myself under exam conditions and see exactly where I stand.

Exams are distinct from lesson quizzes: a quiz teaches (instant explanations, no stakes); an
exam **measures** (marks, no help until the end).

- [ ] Every module offers three question sets: **Foundation** (easy), **Standard** (medium),
      **Challenge** (hard) — selectable before starting
- [ ] Questions carry **marks (1–3)**, weighted by difficulty; the paper shows marks per
      question and a total (e.g. *“Q7 — 2 marks”*, *“Total: 30 marks”*)
- [ ] Exam conditions: no explanations mid-exam, questions answerable in any order, an
      answer-sheet overview showing answered/unanswered, explicit **Submit**
- [ ] Optional countdown timer (off by default); running out submits what is answered
- [ ] **Results page**: marks scored / total, percentage, grade band
      (*Distinction ≥ 80 · Merit ≥ 70 · Pass ≥ 55 · Not yet*), per-topic breakdown, and a
      full review of every question with the explanation and the student’s answer
- [ ] Results history in `localStorage`: every attempt with date, set, marks; best and most
      recent surfaced; included in the US-603 export
- [ ] Passing Standard *suggests* Challenge (never locks it); failing suggests the specific
      lessons to revisit, like US-405
- [ ] Draws from the same §6.2 question bank via the existing `difficulty` field —
      easy = difficulty 1, medium = 2, hard = 3 — so exam papers grow automatically as the
      bank grows

---

## E5 — Worksheets

### US-501 · Printable worksheets *(Should, M)* · ⬜
**As a** student **I want** a paper worksheet **so that** I can practise away from the screen.

- [ ] One per module, gathering that module's exercises
- [ ] Print stylesheet: no nav, no colour backgrounds, page breaks respected
- [ ] Space to write; question numbers and mark allocations
- [ ] Header with name and date fields

### US-502 · Model answers *(Should, M)* · ⬜
- [ ] A separate answers page per worksheet, not linked from the worksheet itself
- [ ] Full worked solutions with commentary, not bare answers
- [ ] Marked with the lesson each answer draws on

### US-503 · Programming challenges *(Could, M)* · ⬜
**As a** student **I want** bigger tasks **so that** I build something real rather than fragments.

- [ ] 3–5 per module: brief, required behaviour, worked solution
- [ ] Stretch goals for confident students
- [ ] Sample input/output so students can self-check in their own IDE

---

## E6 — Progress & motivation

### US-601 · Progress store *(Must, M)* · ✅ done — *versioned localStorage store*
- [ ] Typed `localStorage` wrapper; versioned schema with migrations
- [ ] Records per lesson: state, quiz attempts, best score, last visited
- [ ] Handles storage being unavailable or full without breaking the site
- [ ] Never stores anything personally identifying

### US-602 · Progress dashboard *(Must, M)* · ⬜ — *home shows a completion count only*
- [ ] Overall completion; per-module bars; total time invested
- [ ] Quiz average and trend over time
- [ ] Weakest topics, ranked
- [ ] Next recommended lesson

### US-603 · Export / import progress *(Must, S)* · 🟡 partial — *export/reset functions exist in code; no UI*
**As a** student **I want** to move my progress between devices **so that** clearing my browser
does not erase months of work.

- [ ] Export downloads `learncsharp-progress.json`
- [ ] Import validates and merges, keeping the better score on conflicts
- [ ] "Reset all progress" behind a confirmation

### US-604 · Streaks and milestones *(Could, S)* · ⬜
- [ ] Consecutive-day streak counter
- [ ] Badges: first lesson, first module, all quizzes ≥80%, course complete
- [ ] Encouraging, never punishing — losing a streak is not shamed

### US-605 · Weak-topic revision *(Should, M)* · ⬜
**As a** student **I want** a quiz built from what I keep getting wrong **so that** revision time
goes where it is needed.

- [ ] `/revise` builds a mixed quiz from topics scoring below 70%
- [ ] Spaced repetition: topics resurface after 1, 3, 7 and 21 days
- [ ] Choose length: 10, 20 or 30 questions

---

## E7 — Reference & revision

### US-701 · Pseudocode → C# reference *(Must, S)* · ⬜
**As a** GCSE student **I want** my exam pseudocode next to the C# **so that** I can translate
what I already know.

- [ ] Full AQA pseudocode table with C# equivalents and links to the teaching lesson
- [ ] Printable

### US-702 · Glossary *(Should, M)* · ⬜
- [ ] Every technical term defined in one sentence, linked to its lesson
- [ ] Alphabetical, filterable, deep-linkable (`/reference/glossary#polymorphism`)

### US-703 · Cheat sheets *(Should, M)* · ⬜
- [ ] One printable card per module: syntax, methods, complexities
- [ ] Big-O table; collection-choice decision table; LINQ operator table

### US-704 · GCSE / A-level mapping *(Could, S)* · ⬜
**As a** tutor **I want** to see which lessons cover which specification points **so that** I can
set relevant homework.

- [ ] Table mapping AQA spec points to lessons, both directions

---

## E8 — Quality & accessibility

### US-801 · Responsive *(Must, M)* · ✅ done — *dedicated mobile pass; 44px targets, no horizontal scroll*
- [ ] Usable at 320px through to ultrawide
- [ ] Code blocks scroll horizontally; the page never does
- [ ] Tap targets ≥44px; quizzes comfortable one-handed on a phone

### US-802 · Accessibility *(Must, L)* · 🟡 partial — *ARIA on quiz/accordion/diagrams, reduced-motion honoured; no audit run*
- [ ] WCAG 2.1 AA: contrast, focus order, visible focus rings
- [ ] Every interaction reachable by keyboard alone
- [ ] Correct ARIA on quizzes; results announced to screen readers
- [ ] Respects `prefers-reduced-motion`; zero automated axe violations

### US-803 · Performance *(Must, M)* · 🟡 partial — *75KB gz initial / +102KB lesson vs 50KB target; Shiki+prerender fixes identified in §3.3*
- [ ] Lighthouse ≥95 in all four categories
- [ ] Lesson pages ship <50KB of JavaScript
- [ ] Largest Contentful Paint under 1.5s on a mid-range phone
- [ ] Fonts self-hosted; no third-party requests at runtime

### US-804 · Dark mode *(Should, S)* · ✅ done — *exceeded: six themes with a picker, no-flash load*
- [ ] Follows the OS by default, with a manual override that persists
- [ ] No flash of the wrong theme on load
- [ ] Code themes swap with the site theme

### US-805 · Offline / installable *(Could, M)* · ⬜
**As a** student **I want** the site to work on the train **so that** a bad signal does not stop
me studying.

- [ ] Service worker caches visited lessons and the full syllabus
- [ ] Installable as a PWA
- [ ] Quizzes and progress work fully offline

---

## E9 — Authoring & CI

### US-901 · Content authoring guide *(Should, S)* · ⬜
- [ ] `CONTRIBUTING.md`: how to add a lesson, a quiz, a worksheet
- [ ] Lesson and question templates to copy
- [ ] House style: British English, second person, no jargon before it is defined

### US-902 · Continuous integration *(Must, M)* · 🟡 partial — *dotnet build + all-lessons run + site build gate every PR; no link/axe/Lighthouse checks*
- [ ] On every PR: `dotnet build`, `dotnet run -- all`, `astro build`, link check, axe, Lighthouse
- [ ] Content schema validation and quiz validation
- [ ] Any failure blocks the merge

### US-903 · Code-snippet verification *(Must, M)* · 🟡 partial — *capture regenerated every build; snippet-to-source tracing not enforced*
**As an** author **I want** proof that every snippet compiles **so that** no student is taught
something that does not work.

- [ ] Each `csharp` block traceable to a compiling file in the console project
- [ ] Blocks that are deliberately wrong tagged `csharp title="will not compile"` and excluded
- [ ] Captured output regenerated on every build; a stale capture fails CI

---

## E10 — Diagrams & visualisation

### US-1001 · Diagram component and pipeline *(Must, M)* · ✅ done — *as a typed component registry + ```diagram fences rather than SVG files*
**As an** author **I want** one way to put a diagram in a lesson **so that** all 100+ diagrams
look and behave alike.

- [ ] `<Diagram>` component: SVG source, caption, required `alt`, optional zoom-on-click
- [ ] SVGs live in `web/src/diagrams/` and are inlined at build time (no extra requests)
- [ ] Every diagram uses CSS custom properties for colour, so it themes automatically
- [ ] Build fails if a diagram is missing an `alt` description
- [ ] Legible at 320px; scrolls inside its own container rather than breaking the page

### US-1002 · Mermaid at build time *(Should, S)* · ⬜
**As an** author **I want** to write flowcharts as text **so that** simple diagrams are quick to
produce and easy to review in a pull request.

- [ ] ```mermaid fences render to static SVG during `astro build`
- [ ] **Zero** Mermaid JavaScript reaches the browser
- [ ] Light and dark variants generated from the site tokens

### US-1003 · Core diagram set *(Must, XL)* · 🟡 partial — *13 of ~110 delivered*
**As a** student **I want** a picture of every structural idea **so that** I can see what the
words mean.

- [ ] ~100 diagrams delivered, per the inventory in §8.3
- [ ] Each is referenced from the paragraph it explains, not floated at the end
- [ ] Each has a caption stating the takeaway in one sentence

### US-1004 · Algorithm steppers *(Should, L)* · ⬜
**As a** student **I want** to step through an algorithm one operation at a time **so that** I
can watch it work instead of imagining it.

- [ ] Play / pause / step-forward / step-back / reset, plus a speed control
- [ ] Covers: bubble · insertion · merge · quicksort · binary search · BFS · DFS · Dijkstra ·
      BST insert and delete · hash-table collisions · circular-queue wraparound · recursion tree
- [ ] The current line of C# is highlighted alongside the visualisation
- [ ] Fully keyboard-operable; honours `prefers-reduced-motion` by starting paused
- [ ] Under 10KB of JS per stepper, lazy-loaded only when scrolled into view

### US-1005 · Diagrams in quizzes *(Could, M)* · ⬜
**As a** student **I want** to be asked questions *about* diagrams **so that** I have to read them
properly.

- [ ] `diagram-mcq`: "after this insert, what does the tree look like?" with four SVG options
- [ ] `label-the-diagram`: drag labels onto a memory layout or tree
- [ ] Both keyboard-accessible

### US-1006 · Printable diagrams *(Could, S)* · ⬜
- [ ] Diagrams print cleanly in worksheets and cheat sheets — black on white, no lost detail
- [ ] Steppers print as a static "key frames" strip

---

## E11 — Audience pathways

### US-1101 · Pathway chooser *(Must, M)* · ⬜
**As a** first-time visitor **I want** to say who I am **so that** the site starts me in the right
place at the right depth.

- [ ] First visit asks: GCSE (Y9–11) · A-Level (Y12–13) · Professional · Just browsing
- [ ] Choice stored in `localStorage`; changeable any time from the header
- [ ] Each option shows its route, its length and where it starts
- [ ] Dismissible — "just browsing" gives the full syllabus with nothing hidden

### US-1102 · Pathway-aware ordering *(Must, M)* · ⬜
**As a** student **I want** "next lesson" to follow *my* route **so that** I am never sent
somewhere irrelevant.

- [ ] Prev/next, "continue" and progress all respect `pathwayOrder`
- [ ] Lessons off your pathway remain fully reachable, marked "not on your route"
- [ ] Progress is measured against your own pathway, not all 62 lessons
- [ ] Switching pathway re-maps progress rather than discarding it

### US-1103 · Depth toggles *(Should, M)* · ⬜
**As a** professional **I want** to skip the basics inside a lesson **so that** I am not made to
read what a variable is.

- [ ] `<Depth level="foundation|standard|deep">` sections in lesson content
- [ ] Foundation blocks collapse by default for professionals; expand for GCSE
- [ ] "Go deeper" blocks expand by default for A-Level and professionals
- [ ] The reader can always override, and the choice sticks

### US-1104 · Fast track for experienced developers *(Should, L)* · ⬜
**As a** developer who already knows Java or Python **I want** a two-week route **so that** I
learn what is *different* rather than what is the same.

- [ ] `/fast-track`: ~12 lessons covering the genuinely C#-specific material
- [ ] Assumes loops, types and functions are already understood
- [ ] Leads with: properties, LINQ, async, records, pattern matching, DI, nullable refs
- [ ] Ends with a "you are ready" checklist and pointers into the deep material

### US-1105 · Language-transfer guides *(Should, M)* · ⬜
- [ ] `/reference/from-java` and `/reference/from-python`
- [ ] Side-by-side tables of the same task in both languages
- [ ] The traps specifically: Java's `==` on strings vs C#'s · Python's dynamic typing vs `var` ·
      checked exceptions · properties vs getters · LINQ vs streams/comprehensions

### US-1106 · Specification coverage *(Should, M)* · ⬜
**As a** teacher **I want** to see which lessons cover which exam points **so that** I can set
work that matches the specification.

- [ ] AQA **GCSE** 8525 mapped to lessons, both directions
- [ ] AQA **A-Level** 7517 mapped to lessons, both directions
- [ ] Gaps stated honestly where the site goes beyond, or does not cover, a spec point
- [ ] Printable

### US-1107 · Audience-tagged questions *(Should, M)* · ⬜
**As a** student **I want** questions pitched at my level **so that** they are challenging but
not crushing.

- [ ] Questions tagged `gcse` / `alevel` / `professional`, and by difficulty 1–3
- [ ] Quizzes draw from the tags matching your pathway
- [ ] A-Level quizzes include exam-style extended-answer questions
- [ ] Professional quizzes lean on `predict-output` and `spot-the-bug`

### US-1108 · NEA / project guidance *(Could, M)* · ⬜
**As an** A-Level student **I want** help with my coursework **so that** I can apply this to my
NEA project.

- [ ] A guide to choosing and scoping an NEA project in C#
- [ ] How to evidence design, testing and evaluation
- [ ] Worked example carried through analysis → design → implementation → testing

---

## 8. Core topic coverage

The 31 topics every C# student is expected to know, each mapped to the lesson that teaches it.
**Every one is covered.** Five needed new lessons — those became Module 6.

| # | Topic | Lesson(s) | Status |
|---|---|---|---|
| 1 | Encapsulation | **2.4** | ✅ written |
| 2 | Inheritance | **2.5** | ✅ written |
| 3 | Polymorphism | **2.6** | ✅ written |
| 4 | Collections | 3.1, 3.2, 3.3 | ✅ written |
| 5 | Exceptions | 1.11, 4.7 | ✅ written |
| 6 | Generics | 3.4 | ✅ written |
| 7 | Delegates | 4.1 | ✅ written |
| 8 | Tuples | 4.6 | ✅ written |
| 9 | String interpolation | 1.3, 1.8 | ✅ written |
| 10 | LINQ | 3.6, 3.7 | ✅ written |
| 11 | IO | 4.9 | ✅ written |
| 12 | Async programming | 4.10 | ✅ written |
| 13 | **Threading** | **6.1** | 🆕 **new lesson needed** |
| 14 | Attributes | 4.11 | ✅ written |
| 15 | **Serialization** | 4.9 (JSON basics) + **6.4** | 🟡 **needs depth** |
| 16 | Reflection | 4.11 | ✅ written |
| 17 | Memory management | 4.8 | ✅ written |
| 18 | **Performance** | 5.1 (Big-O) + **6.2** | 🟡 **needs a dedicated lesson** |
| 19 | **Security** | **6.3** | 🆕 **new lesson needed — nothing covers it today** |
| 20 | Pattern matching | 1.5, 4.6 | ✅ written |
| 21 | Records | **2.9** | ✅ written |
| 22 | Interfaces | **2.7** | ✅ written |
| 23 | Extension methods | 4.4 | ✅ written |
| 24 | **Dependency injection** | 2.13 (principle) + **6.5** | 🟡 **needs a real container** |
| 25 | Events | 4.3 | ✅ written |
| 26 | Iterators | 3.5 | ✅ written |
| 27 | Nullable types | 4.5 | ✅ written |
| 28 | Structs | **2.8** | ✅ written |
| 29 | Enums | **2.8** | ✅ written |
| 30 | Operator overloading | **2.10** | ✅ written |
| 31 | Abstract classes | **2.3, 2.6** | ✅ written |

### 8.1 Module 2 — OOP foundations *(expanded)*

The original module taught the *C# syntax* for OOP but assumed the concepts. Six lessons were
added so the ideas are taught properly, and the module now runs **2.1 → 2.15**. All fifteen are
**written and running**.

| # | Lesson | Status |
|---|---|---|
| **2.1** | **What is object-oriented programming?** — procedural vs OO on the same problem, nouns→classes/verbs→methods, the four pillars, when *not* to use OOP | 🆕 written |
| 2.2 | Classes, objects, fields, constructors | ✅ |
| **2.3** | **Abstraction** — what vs how, abstraction vs encapsulation, levels of abstraction, leaky abstractions | 🆕 written |
| 2.4 | Encapsulation, properties, access modifiers | ✅ |
| 2.5 | Inheritance | ✅ |
| 2.6 | Polymorphism, virtual/override/abstract | ✅ |
| 2.7 | Interfaces | ✅ |
| 2.8 | Value vs reference types, structs, enums | ✅ |
| 2.9 | Records and immutability | ✅ |
| 2.10 | ToString, Equals, GetHashCode, operators | ✅ |
| 2.11 | Static members and composition | ✅ |
| **2.12** | **Class relationships and UML** — is-a/has-a/uses-a, association, aggregation vs composition, multiplicity, reading a class diagram and turning it into code | 🆕 written |
| **2.13** | **Coupling, cohesion and SOLID** — each principle with a before/after, plus DRY, YAGNI and the Law of Demeter | 🆕 written |
| **2.14** | **Design patterns** — Strategy, Factory, Observer, Singleton (with its health warning), Repository, Adapter, Decorator | 🆕 written |
| **2.15** | **Modelling workshop** — a full school-library system built from a written brief, then critiqued | 🆕 written |

> 2.12 is where the diagram requirement pays off most: UML is a *visual* notation, so that lesson
> is diagram-led by nature.

### 8.2 Module 6 — Production C# *(new)*

The five gaps become one coherent module about **writing code other people depend on**. It sits
after the algorithms module and before the capstone projects.

| # | Lesson | Covers |
|---|---|---|
| 6.1 | **Threading and concurrency** | `Thread` vs `Task`, race conditions demonstrated live, `lock`, `Interlocked`, deadlock and how to avoid it, `ConcurrentDictionary`, `Parallel.For`, when concurrency is *not* worth it |
| 6.2 | **Performance and benchmarking** | Measure, never guess. `Stopwatch` vs BenchmarkDotNet, allocation and GC pressure, boxing, `StringBuilder`, `Span<T>` and `stackalloc`, struct vs class, capacity hints, premature optimisation |
| 6.3 | **Security essentials** | Validate all input, parameterised queries vs SQL injection, hashing passwords properly (salt, bcrypt/Argon2 — never MD5/SHA alone), `RandomNumberGenerator` vs `Random`, secrets out of source control, least privilege, dependency risk |
| 6.4 | **Serialization in depth** | `JsonSerializerOptions`, naming policies, custom converters, polymorphic payloads, schema versioning and back-compat, XML, and why binary formatters are banned |
| 6.5 | **Dependency injection and architecture** | From hand-wired constructors (2.9) to `IServiceCollection`, the three lifetimes, `IOptions`, composition root, why DI makes testing trivial, and when a container is overkill |

Module 7 (the four capstone projects) is unchanged apart from renumbering from 6.x to 7.x —
**already done in the codebase.**

### 8.3 Diagram inventory

Roughly 100 diagrams. This is the working list, by module.

| Module | Diagrams | Type |
|---|---|---|
| **1 Foundations** | compile → IL → JIT → machine code · variable as a labelled box · type sizes to scale · integer division discarding the remainder · flowcharts for `if`/`while`/`do-while`/`for` · array indexing from 0 · out-of-bounds · 2D grid vs jagged rows · string immutability (old string orphaned) · `+=` in a loop vs StringBuilder buffer · method call and return · value vs reference parameter passing · exception unwinding the call stack | SVG + Mermaid |
| **2 OOP** | procedural spaghetti vs objects side by side · nouns→classes/verbs→methods · the four pillars on one card · **UML class-diagram notation** · association vs aggregation vs composition (hollow vs filled diamond) · multiplicity · coupling tight vs loose · cohesion high vs low · each SOLID principle before/after · Strategy/Factory/Observer/Decorator structure diagrams · the library model built up step by step · class blueprint → many objects · encapsulation as a capsule with a controlled gate · inheritance tree (Animal → Dog/Cat) · constructor order base-first · polymorphic dispatch: one call, many bodies · abstract vs concrete · one class implementing many interfaces · **stack vs heap for value and reference types** · two references to one object · `with` producing a copy · static shared by all instances · composition (Car *has-a* Engine) vs inheritance | SVG |
| **3 Collections** | List capacity doubling and the copy cost · Dictionary key → hash → bucket · HashSet uniqueness · stack push/pop · queue enqueue/dequeue · priority queue ordering · linked list nodes and arrows · generic type substitution · **lazy LINQ pipeline** — items flowing one at a time through Where → Select · deferred vs immediate execution timeline · GroupBy buckets · Join matching keys | SVG + stepper |
| **4 Advanced** | delegate as a pointer to a method · multicast invocation list · closure capturing a variable · event publisher → subscribers · extension method resolution · nullable flow analysis · tuple deconstruction · exception type hierarchy · **stack, heap and GC generations 0/1/2** · boxing copying a struct to the heap · `using` scope and disposal · file stream vs read-all-into-memory · **async timeline: blocking vs awaiting vs WhenAll** · reflection reading an assembly's own metadata | SVG + Mermaid |
| **5 DS & Algorithms** | **Big-O growth curves on one chart** · linear vs binary search step counts · bubble/insertion/selection passes · merge sort divide-and-conquer tree · quicksort partitioning around a pivot · recursion call tree for `Factorial` and `Fibonacci` · the exponential blow-up of naive Fibonacci · call stack growing and unwinding · singly vs doubly linked nodes · **circular queue wrapping round** · BST shape, and the same keys inserted sorted (degenerate) · the four traversal orders on one tree · graph as nodes and edges · BFS rings vs DFS depth-first path · Dijkstra relaxing edges · hash table buckets, chaining and probing · load factor and resize | SVG + steppers |
| **6 Production** | thread timeline and an interleaved race condition · `lock` serialising access · deadlock as a cycle · `Parallel.For` partitioning · benchmark bar charts · allocation and GC pressure · password + salt → hash (and why fast hashes are wrong here) · SQL injection vs a parameterised query · object → JSON → object round trip · schema versioning · DI container object graph · service lifetimes over time | SVG + Mermaid |
| **7 Projects** | architecture diagram per project · guessing game as binary search on the number line · grade manager data flow · text adventure map as a graph · to-do app layering (UI → service → store) | Mermaid |

---

## 9. Content inventory

Already written and verified in the console project — this is the raw material.

| Module | Title | Lessons | Written | Quiz Qs | Test Qs |
|---|---|---|---|---|---|
| 1 | Foundations | 11 | ✅ 11 notes · 11 code | 110 | 30 |
| 2 | Object-oriented programming | **15** | ✅ 15 notes · 15 code | 150 | 40 |
| 3 | Collections, generics, LINQ | 7 | ✅ 7 notes · 7 code | 70 | 25 |
| 4 | Advanced C# | 11 | ✅ 11 notes · 11 code | 110 | 30 |
| 5 | Data structures & algorithms | 9 | ✅ 9 notes · 9 code | 90 | 30 |
| 6 | **Production C#** | **5** | ⬜ 0 notes · **0 code — to write** | 50 | 20 |
| 7 | Mini projects | 4 | ✅ 4 notes · 4 code | 40 | 15 |
| | **Total** | **62** | **57 / 62 notes · 57 / 62 code** | **620** | **190** |

**Every written lesson now has both code and notes** — 57 of 62, verified by
`dotnet run -- all` on every CI run. Module 6 (code + notes) is all that remains of the
lesson content. The outstanding bulk is **questions (610 of 620)** and **diagrams (~97)**.

---

## 10. Milestones

### M0 — Walking skeleton · ✅ **complete**
US-101, 102, 103, 202, 206 · Lesson 1.4 renders end to end from verified output with a working
quiz and localStorage progress — **and the site is live on Azure**, deploying on every merge.

### M1 — Module 1 vertical slice · 🟡 **mostly done — blocked on questions**
US-104, 203, 205, 206, 207, 301–303, 305, 401–404, 601, 602, **1001, 1002, 1101, 1102**
**Done when:** a student can complete all 11 Module 1 lessons with quizzes and see progress.
Reading, navigation and progress all work; **only lesson 1.4 has a quiz**, so the
lesson-completion loop exists for one lesson in eleven. Writing Module 1's ~110 questions is
what closes this milestone — then put it in front of a real student.

### M2 — Full content · 🟡 **theory done; assessment not started**
US-204, 405, 406, 501, 502, **1003, 1107** · **Done when:** all 62 lessons, 620 quiz questions,
~110 diagrams, 7 worksheets and 7 module tests are live.
**57/62 lessons fully written and live** (Modules 1–5, 7 with notes, code, verified output);
13/~110 diagrams. Remaining: Module 6 (five lessons), 610 questions, worksheets, module tests.

### M3 — Retention & revision · ⬜ *(2 weeks)*
US-603, 605, 701, 702, 703, **1004, 1103, 1104, 1105, 1106** · **Done when:** weak-topic
revision, export/import, the reference section, the fast track, the transfer guides and the
algorithm steppers all work.

### M4 — Polish · ⬜ *(2 weeks)*
US-304, 604, 704, 801–805, 901–903, **1005, 1006, 1108** · **Done when:** Lighthouse ≥95, zero axe
violations, offline-capable, CI green.

**Total: ~19 weeks** (Module 6, the expanded OOP module, diagrams and the three pathways). M1 is the critical milestone — everything after it is repetition of a
proven pattern.

---

## 11. Deferred to v2

| Idea | Why later |
|---|---|
| In-browser C# execution (Blazor WASM + Roslyn) | 10–20MB payload; revisit once content is proven |
| Accounts and cloud sync | Needs a backend; breaks "purely static" |
| Tutor dashboard and class management | Depends on accounts |
| Adaptive difficulty | Needs usage data we do not have yet |
| A-level / further modules | Finish GCSE → advanced first |
| Translations | Content must stabilise first |

---

## 12. Definition of done

A **story** is done when: acceptance criteria pass · keyboard and screen-reader tested ·
responsive 320px–1920px · light and dark verified · no new CI failures · Lighthouse not regressed.

A **lesson** is done when: frontmatter validates · every snippet traces to compiling code ·
output captured from a real run · **every structural or temporal concept has a diagram, each with
a real `alt` description and correct in both themes** · ≥10 quiz questions with explanations ·
exercises with model answers · read end-to-end for tone and reading age.

---

## 13. Open questions

1. **Reading age** — target 13–14. Should we measure it, or review by eye?
2. **Worksheet format** — print stylesheet only, or generated PDFs too?
3. **Question authoring** — hand-write all 510, or draft from the lesson code and edit?
4. **Domain** — GitHub Pages subdomain, or a custom domain?
5. **Analytics** — none at all, or privacy-preserving counts to find where students drop out?
6. **Diagram authoring** — hand-write SVG, or draw in Figma/Excalidraw and export? Hand-written
   SVG themes properly and diffs readably in git, but is slower to produce.

---

## 14. Next actions

1. ~~Sign off this plan~~ · ~~scaffold `web/`~~ · ~~output capture~~ · ~~ship 1.4 end to end~~ ·
   ~~write all Module 1–5 and 7 notes~~ · ~~deploy to Azure Free~~ — **done, live**.
2. **Question bank for Module 1** (~110 questions, US-406) — closes M1 and unlocks testing with
   a real student. *← current task*
3. Question bank for Modules 2–5 and 7.
4. Write the five Module 6 lessons — code first, then notes (US-204).
5. Remaining question types beyond multiple-choice (US-403), then module tests (US-405) and
   **topic exams with marks and graded sets (US-407)** — the bank from steps 2–3 feeds both.
6. Progress dashboard + export UI (US-602, US-603).
7. Diagrams tranche 2 (US-1003) and the bundle-size fixes from §3.3 (US-803).
