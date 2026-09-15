# African Taste — standard button style

The glossy pill button is the standard for every button in the game.

## Use it

Drag **`PillButton.prefab`** into a Canvas. Then:

1. **Colour** — set the root Image's sprite:
   - `UI_PillOrange` — primary / confirm (Main Menu PLAY)
   - `UI_PillRed` — destructive / quit (Main Menu EXIT)
   - `UI_PillWhite` — neutral; tint the Image colour for any new colour
2. **Label** — type into the `Label` child. Keep it white and centred.
3. **Icon** — the `Icon` child is disabled by default. Enable it for buttons that
   need a glyph (e.g. the white play triangle) and it sits to the left of the label.
4. **onClick** — wire it like any Unity Button. The prefab ships with an empty list.

## Rules

| Rule | Value | Why |
|---|---|---|
| **Height** | **always 150** | The sprite is authored at 150px and 9-slices **horizontally only** (`border {x:73, y:0, z:73, w:0}`). Any width is safe; changing the height stretches the caps, gloss and shadow. |
| Width | free, ≥ 240 (440 on the Main Menu) | Must exceed 2 × 73 so the middle slice exists. |
| Label offset | y = **+8** | The sprite's visible body occupies the top 134px of its 150px canvas — the rest is drop shadow — so its optical centre is 8px above the rect centre. Icon and label both sit at +8. |
| Font | Bangers SDF, 72pt, bold, white | Matches the Food Menu's display type. |
| Icon ↔ label | ≥ 30px clear (icon box 108×108) | Never let the glyph touch the text. |
| Raycast | root Image `true`, Icon and Label `false` | Only the button body should catch clicks. |

## Sizing note

The rect is **taller than the button looks**: the bottom 16px of the sprite is
drop shadow. A 440x150 rect draws a visible pill of **427 x 134**. Size against
the visible figure, not the rect.

## State feedback

Colour-tint transition, set on the prefab:

- Normal `1,1,1,1` — the authored colour shows exactly
- Highlighted `0.96` — subtle hover lift
- Pressed `0.80` — clear press
- Disabled `0.6, 0.6, 0.62 @ 65%`
- Fade `0.08s`

## Adding a new colour

Everything is generated from one script — see the button sprite generator used to
author these (`make_pill.py` in the task scratchpad). Add one entry to its
`PALETTE` and re-run; it emits a matching pill with the same border, gloss,
extrusion and shadow so new colours stay on-style.

Base colours currently in the set:

- Orange `255,167,38` — preserved from the original Main Menu PLAY button
- Red `239,83,80` — preserved from the original Main Menu EXIT button
- White `232,232,236` — neutral, meant to be tinted
