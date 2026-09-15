# Level One customer system

Replaces the old customer visuals. Every rule below exists to fix a specific
defect recorded in the read-only customer audit.

## Customers

Level One has **two** customers:

```
Customer 1 (female, 'Walking_0')  ->  Customer 2 (male, 'Walking2-removebg-preview_0')  ->  LEVEL COMPLETE
```

Customer 2's `nextCustomer` is null, which is what makes it the last customer:
`first customer.cs` `Leave()` loads the LevelComplete scene when `nextCustomer`
is null, at the end of the walk to `exitX`.

The former Customer 3 (`walk w rmv_0`) was removed from the scene. Its generated
art — `Child_Walk/Idle/Angry.png`, the matching `.anim` clips and
`Customer_Child.controller` — is still on disk but referenced by nothing. Delete
it if you are sure no other level will use it.

## Rules (do not break these when adding art)

| Rule | Fixes |
|---|---|
| `spritePixelsToUnits: 100` on **every** customer and plate sheet | customer shrank ~25 % when it picked up food (carry art was imported at PPU 70) |
| one frame box per character, shared by **all** its clips | frame-to-frame width wobble (old `Angry.anim` swung 66 → 130 px) |
| character drawn at the **same pixel height** in every clip | grow/shrink between Walk / Idle / Angry / Carry |
| `alignment: 9` + `pivot {x: 0.5, y: <ground>}` identical on every slice | vertical/horizontal teleport on state change (old customer 3 mixed a centre pivot with a bottom-left pivot) |
| no `m_ScaleCurves` / `m_PositionCurves` in any clip | animation can never move or resize the Transform |
| **uniform** `m_LocalScale` (5, 5, 1) on every customer | seven of the old nine scales were non-uniform, stretching the art up to +43 % |

## Measurements

Level One camera: orthographic, size 21 → visible world area 74.7 × 42.0 units.
The counter sprite (`table`) occludes everything below **world y = +0.12**, so the
customers are aligned by head clearance, not by a shared floor line.

| | art height | world height | Transform scale | ground y | head top |
|---|---|---|---|---|---|
| Female (customer 1) | 620 px | 31.0 | 5 | −19.6 | +11.4 |
| Male (customer 2) | 620 px | 31.0 | 5 | −19.6 | +11.4 |

Frame margins: 28 px left/right (head-room for the ±2.2° walk lean), 20 px top,
8 px bottom. The pivot sits on the 8 px line, so it is exactly the feet.

## Hierarchy

```
Customer                 FirstCustomer, SpriteRenderer, Animator, CustomerCarry
 └── CarryFood           positioning node; its scale sets the plate size
      └── FoodSprite     SpriteRenderer, starts disabled, z = −0.01
```

`FoodSprite` sits at sorting order 0 like the customer and is pulled 0.01 units
towards the camera, so it draws in front of the body but still goes behind the
counter (z = −0.165) as the customer walks out. Never give it a higher sorting
order or the plate will punch through the counter.

## Animator

Parameters — these types are what `first customer.cs` actually calls, so the
Level Three `isIdle` Trigger/Bool mismatch cannot repeat here:

`isIdle` Bool · `isCarrying` Bool · `carryingFood` Int · `isHappy` Trigger · `isAngry` Trigger

States: **Walk** (default) · Idle · Angry · Happy · WalkWithFood.
`Happy` and `WalkWithFood` reuse the Idle and Walk clips — the carried dish is a
real child object now, so no per-dish body animation is needed. There is no
`IdleWithFood` state because the Level One flow never enters one; adding
unreachable states is exactly what broke the old controllers.

`carryingFood` is still declared so the existing `SetInteger` call keeps working,
but **no transition reads it**. That is what stops a Level One customer walking
away with Nigerian jollof rice.

## Carried-food mapping

| player picked | `selectedFood` | `LevelOneDish` | plate |
|---|---|---|---|
| Ugali | 0 | `Ugali` | `Serve_Ugali.png` |
| Anchoves | 0 | `Anchoves` | `Serve_Anchoves.png` |
| Potato Leaves | 1 | `PotatoLeaves` | `Serve_PotatoLeaves.png` |
| Ugali + Tembele (combo, plate 2) | 4 | `UgaliTembele` | `Serve_UgaliTembele.png` |

`selectedFood` alone cannot distinguish Ugali from Anchoves (both are 0), and the
delayed `Invoke(HideMenu, 5f)` can rewrite it after the player has chosen. So the
plates call `CustomerMenuManager.LatchServedDish()` at the instant they commit a
serve, freezing the identity. `Plate2` passes the dish explicitly, because
`isUgaliTembelePlate` is set by the cooking system itself
(`PrepareForUgaliTembele` / `PrepareForTembeleOnly`) and is therefore the most
truthful record of what is physically on the plate.

`selectedFood`, `isComboOrder`, the ticks, the buttons and every cooking path are
unchanged — `LevelOneDish` is purely additive.

## Orientation

`first customer.cs` no longer does `spriteRenderer.flipX = flipXWhenCarrying`.
Facing comes from the direction of travel only (`Face()`), called when walking in,
when picking up the food, and when leaving. `artFacesRight` is off because the
customer art faces the camera, so the customer is never mirrored while walking
left. `flipXWhenCarrying` and `carryingYOffset` are kept but unused.
