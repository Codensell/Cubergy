## GDD (MVP) — EN
#### Working title

Cubergy

#### Genre & Platform

Genre: 3D arcade / collect → transform → shoot

Platform: WebGL

Session length: 2–5 minutes

#### Fantasy

You start as a small cube, collect energy cubes, build yourself into a cube robot, then into an armed form where you literally become the weapon.

#### Core loop

Move → collect cubes → transform → (repeat) → unlock weapon → (later) eliminate enemies → win.

#### Camera

Third-person: behind the player, slightly above, angled.

Smooth follow.

#### Controls

WASD — movement on a plane.

Mouse — FPS mouse-look (Counter-Strike style), crosshair fixed at screen center.

LMB — charged shot (armed form).

#### Player forms (progression)
Form 0 — Hover Cube

Visual: small cube prefab.

Animation: subtle up/down bobbing on the visual child only (not root).

Abilities: move, collect.

Threshold: collect 10 energy cubes → transform.

Form 1 — Cube Walker

Visual: simple cube-bot made of primitives (prefab).

Animation: basic legs/hips motion while moving (no complex rig for MVP).

Abilities: move, collect.

Threshold: collect 10 more energy cubes → transform.

Form 2 — Armed Walker

Visual: like Form 1 but larger (scale ~ 1.5) + a central “barrel” (cylinder/module).

Abilities: move, collect, charged shooting.

MVP threshold: shooting unlocked (targets/enemies added later).

#### Collectibles (Energy Cubes)

Object: glowing/emissive cube, rotating/pulsing.

Pickup: trigger collider.

Spawn: fixed points or keep N active in scene (e.g., 15).

#### Charged shooting (Form 2)

Hold LMB → charge increases from 0 to max over chargeTime.

On max → auto-shot and/or shot on release (recommended for MVP: max = auto-shot, early release = shot with current power).

Projectile: simple bolt (sphere/capsule) with trail.

Power affects at least: projectile speed/range (damage optional).

#### UI (minimum)

Counter: Energy: x/10 (current form).

Form indicator: Form: 0/1/2.

Charge slider (visible while holding LMB).

#### Level (blocking)

Primitive-based sci-fi arena: platforms/walls/corridors.

Boundaries: invisible walls to prevent falling out.

Later: replace visuals with asset meshes while keeping colliders.

#### Enemies & Win condition (after MVP core)

Enemies are implemented last.

Jam goal: win by destroying all enemies.

Stretch: NavMesh enemy that also collects cubes, transforms, and shoots when it can see the player.

#### Out of MVP scope

Advanced AI, heavy physics, complex rig/animation pipeline, upgrades/skill trees, multiple weapons.