# 🌙 Milk & Moon

A simple, thoughtful baby tracking application designed to make everyday tracking **quick, intuitive, and easy to use — even with one hand while holding a baby in the other.**

Milk & Moon is also a personal learning project I'm using to deepen my understanding of **Angular, .NET, application architecture, API design, and full-stack development**.

> 🚧 **Work in Progress**
> Milk & Moon is an active learning and portfolio project. The architecture and implementation will continue to evolve as I build, learn, and refactor.

---

## 🎯 Project Goals

Baby tracking should make life easier, not give an already-busy parent another complicated system to manage.

The goal of Milk & Moon is to make common baby activities fast and simple to record while still providing enough information to understand what has happened throughout the day.

A parent or caregiver should be able to:

* Quickly log common baby activities
* See what's happened throughout the day
* Review recent activity
* Edit or remove incorrect entries
* Track useful details without being overwhelmed by unnecessary fields
* Use the application comfortably from a phone or desktop browser
* Operate the app **with one hand while holding a baby in the other**

The goal is intentionally **not** to build every possible baby tracking feature.

I'm focused on building a useful core experience with a clean foundation rather than adding features simply because they can be added.

---

## 🚀 Getting Started

### Prerequisites

You'll need:

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* Node.js — see [`.nvmrc`](.nvmrc) for the expected version
* npm

Clone the repository:

```bash
git clone https://github.com/connorphu/Milk-and-Moon.git
cd Milk-and-Moon
```

### Install Frontend Dependencies

```bash
npm run web:install
```

### Run the API

Starts the ASP.NET Core API and watches for changes:

```bash
npm run api
```

### Run the Angular App

In a separate terminal:

```bash
npm run web
```

### Run the Local Mock API

The Angular application currently uses a local `json-server` instance for development.

In another terminal:

```bash
npm run json-server
```

The mock data is backed by:

```text
Web/db.json
```

For the current frontend experience, you'll typically have the Angular app and `json-server` running together.

---

## Why I Built This

I've spent much of my professional career contributing to established applications where many of the foundational architectural decisions had already been made.

That's valuable experience, but it also made me realize that I wanted more opportunities to practice making those decisions myself.

As I think about the next stage of my career, I've been focusing less on simply learning more frameworks and more on developing the **judgment behind good engineering decisions**:

* How should an application be structured?
* Where should responsibilities live?
* What deserves an abstraction?
* What should stay simple?
* How should frontend and backend boundaries be defined?
* What tradeoffs am I making when I choose one approach over another?
* Will another developer understand what I built and why?

A baby tracker initially seemed like a relatively straightforward application.

But once I started building it, I found there were plenty of meaningful product and engineering decisions hiding underneath what looks like a simple CRUD app.

That's exactly what I wanted.

Milk & Moon gives me a place to revisit **.NET**, continue building with **Angular**, and practice owning an application from the initial product decisions through frontend architecture, APIs, persistence, and deployment concerns.

This repo isn't meant to represent a finished senior-level architecture.

**It's meant to document the process of becoming better at making the decisions behind one.**

---

## 👥 A Real Stakeholder

One thing that makes this project especially useful is that I have a real stakeholder: **my wife**.

She has very specific opinions about what makes a baby tracking app useful, what information is actually worth tracking, and what would make the experience frustrating or cumbersome.

That gives me something more valuable than a made-up feature list: real feedback from someone who would actually use the product.

It also gives me practice with an important part of software engineering that goes beyond writing code:

* Listening to a loosely defined need
* Asking the right follow-up questions
* Understanding the problem behind a requested feature
* Deciding what belongs in the core experience
* Translating user feedback into technical decisions
* Knowing when **not** to build something exactly as requested

For example, one of the guiding requirements is that common actions should be easy to complete **with one hand**.

That sounds like a small UX detail, but it influences navigation, form design, button placement, the amount of information shown at once, and how many steps it takes to log an activity.

Having a real stakeholder helps keep the project grounded in solving an actual problem rather than simply adding technically interesting features.

---

## ✨ Core Tracking Experience

Milk & Moon focuses on four common areas of baby tracking.

### 🍼 Feeding

Track bottle and breastfeeding sessions, including information such as:

* Feeding type
* Milk type
* Bottle size
* Amount consumed
* Breast side
* Start and end time
* Notes

### 😴 Sleep

Track sleep sessions, including:

* Start time
* End time
* Sleep location
* Wake reason
* Notes

### 🧷 Diaper

Track diaper changes and relevant observations, including:

* Diaper type
* Pee color
* Rash
* Rash location
* Time
* Notes

### 🧴 Pumping

Track pumping sessions, including:

* Start time
* End time
* Left-side amount
* Right-side amount
* Notes

---

## 🏠 Today

The **Today** page is designed to be the primary home for everyday interaction with the app.

One of the early product decisions I made was to avoid requiring users to navigate to a dedicated logging page every time they need to record something.

Instead, common actions are available directly from Today.

The goal is to reduce the distance between:

**"Something happened" → "It's logged."**

That matters particularly for this application because the person using it may be feeding a baby, carrying one, or functioning on very little sleep.

Reducing taps isn't just a UI preference — it's part of the product requirement.

---

## 🛠️ Tech Stack

### Frontend

* **Angular 22**
* **TypeScript**
* Standalone components
* Angular Signals
* Server-side rendering (SSR)
* Responsive / mobile-first UI

### Backend

* **ASP.NET Core / .NET 10**
* REST APIs
* Entity Framework Core
* Relational data persistence

### Development

* npm workspace scripts
* `json-server` for local mock data
* Git / GitHub

---

## 🧱 Project Layout

```text
Api/     ASP.NET Core Web API (MilkAndMoon.Api)
Web/     Angular application (SSR-enabled)
```

The `Web` application is organized primarily by feature:

```text
today/
trends/
baby-profile/
trackers/
```

Reusable UI and shared application concerns live within `core` and `shared`.

The frontend follows Angular's **standalone-component, signal-first** style.

More detailed frontend conventions are documented in:

[`Web/.claude/CLAUDE.md`](Web/.claude/CLAUDE.md)

The exact structure may evolve as the application grows. Part of this project is learning when architecture improves clarity and maintainability — and when it simply adds unnecessary complexity.

---

## 🧠 Engineering Approach

### Start With the Problem

I'm trying to avoid choosing patterns simply because they're familiar or considered "best practice."

Instead, I want the needs of the application to create the justification for the architecture.

---

### Keep the MVP Focused

Personal projects can quickly turn into endless feature lists.

I'm intentionally prioritizing the core tracking experience before expanding into more advanced functionality.

The question isn't:

> What else could I add?

It's:

> Does this make the core experience meaningfully better?

---

### Favor Clear Code Over Clever Code

I would rather have straightforward code that another developer can understand quickly than introduce an abstraction simply because it might become useful someday.

Abstraction should solve a problem, not anticipate every possible future one.

---

### Refactor When the Code Gives Me a Reason

Instead of trying to predict the application's final architecture upfront, I'm allowing repeated patterns and actual pain points to show me where abstractions belong.

That means I'm comfortable writing something simply first and refactoring once I understand the problem better.

---

### Keep Responsibilities Clear

As the application grows, I'm paying particular attention to the boundaries between:

* Presentation
* State
* Business logic
* Data access
* API communication
* Persistence

The goal isn't perfect separation for its own sake. It's making the system easier to understand, test, change, and maintain.

---

### Design for Real Usage

Although Milk & Moon is a portfolio and learning project, I want product decisions to be grounded in how someone would realistically use it.

That means prioritizing:

* Fewer taps
* Clear information hierarchy
* Fast data entry
* Mobile usability
* Sensible defaults
* Forgiving editing workflows
* One-handed interaction

---

## 💭 What I'm Learning

The technical skills are important, but the biggest reason I'm building this project is to practice the decisions around them.

### Architecture

How much structure does an application actually need at its current size?

When does organization make the application easier to understand, and when does it become ceremony?

### Angular

When should functionality remain inside a feature?

When has something actually earned its place in `shared`?

When should state live locally versus within a service?

How can Signals be used effectively without introducing unnecessary state-management complexity?

### .NET & APIs

How should the API expose the application's domain?

Where should validation and business rules live?

How should API contracts, frontend models, domain models, and persistence models relate to one another?

### Abstraction

When does repeated code indicate that an abstraction would improve the application?

And when are two things merely similar rather than actually the same responsibility?

### Maintainability

One question I keep coming back to is:

> If another developer joined this project tomorrow, could they understand where something belongs without me explaining it?

That question influences many of the structural decisions I'm making.

---

## 🌱 Personal Motivation

This project is also part of a broader goal I've set for myself professionally.

I've spent some time reflecting on what separates being able to **implement a solution** from being able to **own and design one**.

For me, growing toward senior-level engineering isn't about reaching a point where I know every Angular API, .NET feature, or architectural pattern.

It's about becoming better at:

* Breaking down ambiguous problems
* Making decisions without having every answer
* Understanding technical tradeoffs
* Owning functionality end-to-end
* Identifying unnecessary complexity
* Writing code that other developers can work with
* Explaining why a decision was made
* Recognizing when new information means that decision should change

Building something from scratch exposes gaps that are much easier to overlook when working inside an established system.

That's one of the reasons Milk & Moon exists.

There will almost certainly be code in this repository that I would approach differently six months from now.

I consider that a sign that the project is doing its job.

---

## 🤝 Feedback Welcome

Part of the reason this repository is public is so that the learning process can be public too.

If you're an engineer, architect, or engineering leader who happens to look through the project, constructive feedback is genuinely welcome — particularly around:

* Angular architecture
* .NET architecture
* Separation of concerns
* API design
* Project organization
* State management
* Testing strategy
* Places where I've overengineered
* Places where I haven't engineered enough

I'm interested not only in whether something works, but whether there is a clearer or more maintainable way to think about the problem.

---

## 👨‍💻 Author

**Connor Phu**

Software Engineer focused on Angular, .NET, APIs, cloud technologies, and building maintainable web applications.

[GitHub](https://github.com/connorphu)
