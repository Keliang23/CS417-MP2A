# CS417-MP2A

## Farm scene blockout

Open `Assets/Scenes/Farm.unity` in Unity 6000.5.6f1. The scene is saved with all objects visible and editable in the Hierarchy; no runtime generation is required.

- `sceneObjects/groundAndPaths`: ground and entrance path.
- `sceneObjects/plantingArea`: nine individually editable plots, named `plotRow1Col1` through `plotRow3Col3`.
- `sceneObjects/toolArea`: open entrance, farm fence panels, workbench and supply crate.
- `sceneObjects/boundaryColliders`: four overlapping invisible BoxColliders enclosing the clearing.
- `sceneObjects/forest`: individually editable trees, edge rocks and 60 bushes in two staggered rows. Inner bushes are 1.2–1.55 m tall; outer bushes are 2.3–2.76 m tall. Alternate plants were removed and the outer row reduced in size to avoid a solid hedge appearance. Decorative meshes do not provide boundary collision.
- `sceneObjects/layoutMarkers`: player spawn marker.
- `grabbableObjects`: watering can, harrow and seed bag.

All scene GameObjects use lowerCamelCase, without underscores, spaces or sorting prefixes. Tool prefabs are saved under `Assets/Prefabs/Farm/Tools`. The rake, seed bag and watering can have XR Grab Interactable components and physical Rigidbody components. `previewCamera` is disabled now that `playerRig` provides the view.

Scene-specific URP materials are under `Assets/Materials/Farm`; imported source materials are preserved. The ground and boundary have colliders, but player containment must be tested after adding the XR controller.

The soil (`Tillage_1x1`), fence (`Fence_Middle`), tools, crate and trees use the imported farm packs. Ground, path and workbench still use simple geometry. The blue procedural sky material is derived from the imported Lite Farm Pack skybox. Bush meshes come from [Kenney Nature Kit](https://kenney.nl/assets/nature-kit) (CC0); the three imported FBX files and license are in `Assets/asset/KenneyNature`.

## VR setup status

XR Interaction Toolkit 3.6.0, OpenXR 1.17.1 and the official Starter Assets sample are installed. `playerRig` is a prefab variant saved in `Assets/Prefabs/Farm/Player/playerRig.prefab` and placed at `(0, 0, -5.4)` in Farm, using Floor tracking. It includes the official controller/input/locomotion setup. `xrInteractionManager` is present; the old preview camera is disabled. OpenXR is enabled for Windows/Standalone with the Oculus Touch controller profile. Android deployment is not configured. No MP1B score UI is included.

Headset tracking, joystick movement, boundary collision and controller input still require hardware testing. No teleportable areas have been added; keep teleport destinations inside the farm when implementing them.

## Tilling and planting

1. Grab `harrow` from the tool bench using the controller's grab input. Touch its rake head to a light-brown cell in the 3x3 planting area.
2. The cell creates a `tilledSoil` prefab exactly at its centre and changes to dark brown. Touching the handle or leaving the rake on the ground does not till. Outside the nine cell colliders, no soil can be created.
3. Grab `seedBag` from the same bench and touch it to dark-brown soil. A green `seedling` appears at the centre. The bag is reusable, but each successful planting consumes one seed from the shared resource inventory. Untilled or occupied cells consume nothing; zero seeds prevents planting.
4. Untilled cells reject planting. Repeated contact cannot duplicate soil or seedlings, and re-tilling cannot erase a planted crop. Play Mode restart resets the plots; saving gameplay progress is not implemented.

`FarmCell` owns cell state and prefab spawning; `HarrowTool` and `SeedBagTool` forward contact only while their tool is selected by XRI. References are assigned in the Inspector. Cells, tool contact volumes and source prefabs remain editable. Soil and plant prefabs are in `Assets/Prefabs/Farm/Plants`; visuals reuse the imported Lite Farm Pack with separate state materials.

The seed bag uses a custom low-poly drawstring sack mesh in `Assets/Models/Farm`, with a larger silhouette, rope tie and a SEEDS/sprout label. Its visual children remain editable inside the existing `seedBag` prefab.

Rubric features implemented here: Building Affordance (2), Gridded Buildings (2), and Foundation Buildings (2).

Validation: 18 automated Play Mode checks passed using actual physics contacts and XRI manual selection. These cover held/unheld tools, off-centre grid snapping, out-of-area rejection, till-before-plant, visible material changes and duplicate prevention. Controller ergonomics still need a headset check.

## Watering and crop growth

Grab `wateringCan`, hold the activate/trigger input and position its spout above a tilled grid cell within 1.5 m. A blue stream shows pouring. Release the trigger or the can to stop. This first pass uses unlimited water; there is no refill/inventory system yet. Untilled cells cannot retain water.

`SoilMoisture` stores moisture from 0 to 100. Water adds 40 units/second; drying removes 2 units/second. Wet soil is darker and gradually returns to dry brown. Each plot has its own moisture and renderer property block, so watering one does not recolor its neighbours.

`CropGrowth` advances growth using `growthRate * Time.deltaTime` only while its soil has moisture. Default growth takes 30 wet game seconds: small below 40%, medium from 40%, mature at 100%. The three editable visual children reuse `Plant_Small`, `Plant_Medium` and `Plant_Large`. Drought pauses growth without resetting it; rewatering resumes it. Growth clamps at maturity. The medium stage appears after about 12 wet seconds; maturity after 30. Time uses Unity scaled game seconds, so pause stops growth; there is no day/night clock. Change the seedling prefab CropGrowth growthRate to 100 divided by the desired duration.

`WateringCanTool` handles XRI activation and the stream/raycast. The `farmSoil` physics layer contains only the nine cell targets. Rates, stage references, wet/dry colours, spout and reach are Inspector-editable. New scoring features: Passive Stat Gain (2) and Caretaking Affordance (2), bringing the current feature tally to 14/33 including Interface Metaphor (4).

Validation: 20 Play Mode watering/growth checks passed with XRI selection/activation, plus all 18 tilling/planting regression checks. Headset trigger mapping and pouring ergonomics remain to be checked on hardware. The blue stream is visual feedback, not a UV-animation rubric implementation.

## Harvesting and resources

When a crop reaches maturity, point at it or reach toward it and press the controller grab/select input. `CropHarvest` forwards the XRI selection to its `FarmCell`. Immature plants have no active harvest target. Each successful harvest adds one crop to `FarmResources`, removes the plant, and leaves the soil tilled with its remaining moisture. Use the seed bag again to begin another growing cycle. Duplicate callbacks and callbacks from a removed crop cannot add extra resources or harvest a replacement.

The editable `sceneObjects/toolArea/resourceBoard` displays the shared crop count and short instructions. `ResourceDisplay` only handles presentation and subscribes to resource changes; the count is maintained separately in `FarmResources`. Scene and prefab objects use lowerCamelCase. The harvest collider uses ordinary collision because the current XR Rig's near/far detectors ignore triggers.

Each harvest awards one crop. Restarting Play Mode resets resources and plots. Harvesting Affordance contributed 2 credits, bringing the tally before trading to 16/33. The resource and trading extension below raises the tally after trading to 20/33.

Validation: 18 Play Mode checks passed for maturity restrictions, 30-second growth, pause behaviour, rig-compatible ray detection, XRI selection, immediate UI updates, duplicate prevention, soil retention, replanting and shared counts across plots. Hardware controller ergonomics still need a headset check.

Regression after harvesting: all 18 tilling/planting checks and 20 watering/growth checks passed again.

## Resources and trading

The farm starts with 9 seeds, 5 coins, and zero crops, fertilizer and stones. All five totals appear on the enlarged `sceneObjects/toolArea/resourceBoard`. Point at a trade button or reach toward it and press grab/select once. Holding the selection does not repeat purchases. The three buttons offer:

- Sell 1 crop for 3 coins.
- Buy 1 seed for 1 coin.
- Buy 1 fertilizer for 2 coins.

Insufficient resources reject the exchange without changing either balance, and the board shows what is missing. `FarmResources` owns balances and makes each trade as one complete update; `TradeButton` forwards input and displays the result, while `ResourceDisplay` displays the totals. Starting balances and each button's costs/rewards are serialized Inspector fields. If changing a price, also update its editable child label.

Planting consumes one seed only after the cell has passed its empty/tilled checks. After harvesting, sell the crop, buy a seed and plant again to continue the loop. The green `fertilizerBag` on its own stand is a reusable grabbable tool: touch a planted plot while holding it to spend one fertilizer and add 25 growth points. Each crop can receive fertilizer once; yellow granules mark the application. Empty plots, mature crops, already fertilized crops and an empty fertilizer inventory consume nothing. Replanting resets fertilizer eligibility. Water is still required for normal growth, which remains 30 wet seconds without fertilizer.

Three loose stones beside the plots can be selected to collect one stone each. They reuse the imported `Rock_04` mesh and disappear when collected. Stones are an obtainable inventory resource; large-rock shattering, random spawning and stone trading are not implemented in this step.

Owned Hierarchy names use lowerCamelCase, and the market, fertilizer stand, buttons and loose stones are saved in the scene. The fertilizer tool is a prefab at `Assets/Prefabs/Farm/Tools/fertilizerBag.prefab`. `WorldText` and the `Farm/WorldText` shader make labels respect scene depth, avoiding text showing through the board; the shader includes stereo instancing support.

Scoring added: Resource Content (5 resources = 2) and Crafting Content (3 separate exchanges = 2). Tally after this step: **20/33**. Fertilizing and collecting stones reuse existing Caretaking/Harvesting features and are not counted again.

Validation: 41 Play Mode trading/resource/fertilizer/presentation checks passed, plus 18 tilling/planting, 20 watering/growth and 18 harvesting regression checks (97 total). The market preview was visually inspected after fixing depth-tested text and placement. Physical headset/controller ergonomics still require a hardware check.

## Crop health and death

`CropHealth` on the `seedling` prefab starts with 100 health. A crop can remain on completely dry soil for 10 game seconds without damage. Continued drought then removes 5 health/second, so an otherwise healthy crop dies after roughly 30 continuous dry seconds in total. These rules also apply to mature crops waiting to be harvested.

While soil has moisture, damage stops, the drought grace timer resets, and health recovers at 10/second up to 100. Normal growth still requires moisture and takes 30 wet seconds without fertilizer. Health and drought timing use scaled Unity game time and stop when the game is paused.

Plant renderers in all three growth stages gradually blend from their original green toward yellow as health drops. Per-instance renderer property blocks keep other plots and shared materials unchanged; fertilizer granules are excluded. Recovery restores the original colors. Health, grace duration, damage rate, recovery rate and wilt color are editable on the prefab's `CropHealth` component.

At zero health, the crop is immediately disabled and destroyed, and `FarmCell` clears its planted state without awarding crops. Tilled soil remains in place. Replanting spends one seed and starts a new crop at full health, with fresh drought grace and fertilizer eligibility. Callbacks from a removed crop cannot clear a replacement crop.

Scoring added: Stat Content (growth, moisture and health = 3 distinct stats, 1 point) and Death (2 points). Tally after this step: **23/33**.

Validation: 27 health/drought/death Play Mode checks passed, plus 20 watering/growth, 18 harvesting and 41 trading/resource regression checks (106 total). The yellow-versus-green crop preview was visually inspected. Loose stones also now use the existing URP farm material, correcting their pink rendering.

## Mining, irrigation and upgrades

The workshop on the right of the farm adds a material loop: mine stones, harvest and sell crops for coins, buy kits, then apply those kits with the matching reusable boxes. All workshop geometry, the pickaxe stand, six mining spawn markers and nine planting cells are saved and editable in the scene. Runtime generation is limited to gameplay boulders, fragments and player-built sprinklers. Names remain lowerCamelCase.

### Try the new loop

1. Grab the curved pickaxe from its small stand beside the workshop. Strike a boulder with its head, withdrawing between hits. A dropped tool or a head left inside a rock does not repeatedly mine. The basic pickaxe needs three strikes; cracks and the hardness label show damage.
2. A shattered boulder produces three loose stones. Select each stone to collect it; merely breaking a rock does not add stones to the inventory.
3. Select a workshop purchase button. Each transaction deducts both stone and coin costs together and adds one kit. Insufficient funds change neither balance and produce a message and rejection sound.
4. Grab the matching labelled box from the shelf and touch the target. Boxes are reusable applicators; applying a kit consumes one unit from the corresponding workshop inventory. No purchase means no free build or upgrade. Withdraw and touch again for another target.
5. Build a sprinkler on an empty, already-tilled grid cell. It occupies that cell, preventing planting or another sprinkler there. Place the watering-can spout above its tank and hold activate to refill it.
6. The sprinkler waters only the four cardinally adjacent tilled crop plots. It does not water diagonal cells or other sprinklers. Empty tanks stop their streams and watering. Neighboring crop plots gain moisture while the reservoir loses water.

| Kit | Stone cost | Coin cost | Target and effect |
| --- | ---: | ---: | --- |
| Build / SprinklerKit | 5 | 3 | Empty tilled cell: create a sprinkler with capacity 100, initially empty |
| Pick / PickaxeKit | 3 | 3 | Pickaxe handle: change three strikes per boulder to two |
| Tank / TankKit | 4 | 4 | Sprinkler tank: increase capacity from 100 to 200, preserving current water |
| Soil / SoilKit | 2 | 2 | Tilled crop cell: reduce drying from 2 to 1 moisture/second |

The pickaxe gets a larger curved steel head and brass collar; the sprinkler gets a larger tank with a side reservoir; upgraded plots get individually editable stone edging. Each upgrade is one-time per target and rejects repeat spending. Soil upgrades persist across harvesting, crop death and replanting. The initial 9 seeds and 5 coins remain unchanged, and kits start at zero. The market displays the original five resources; the workshop separately displays all four kit inventories (nine resource types total).

The sprinkler supplies 6 moisture/second to each eligible adjacent plot, consuming 2 reservoir units/second while actually watering. A full basic tank supports about 50 seconds of active watering; the upgraded tank about 100. These are reservoir units, not a one-to-one transfer of soil moisture units. Full or untilled plots do not receive water. A visible tank gauge and water label show remaining supply. Defaults are editable in the sprinkler prefab. Constructing all nine cells as sprinklers would leave no crop space; placement is a farm-layout decision.

Three boulders spawn at random distinct markers at startup. Replenishment attempts happen every 15–25 game seconds, with at most three live boulders. A marker containing uncollected fragments cannot be reused, so deposits and fragments remain bounded by the six markers. Mining markers remain inside the playable clearing and separate from the planting grid.

### Feedback and asset details

Success and failure messages last three game seconds. Tool and plot feedback face the active player camera when available. Upgrade sounds reuse the imported `070_Equip_10.wav` and `033_Denied_03.wav`; mining reuses `39_Block_03.wav`, pitched down for the upgraded pickaxe. Clips are under `Assets/Audio/Leohpaz/RPG_Essentials_Free`; original audio assets are preserved. Spatial AudioSources live on the relevant objects. These are short effects; upgrades do not switch background music.

The curved pick head is an editable generated mesh asset under `Assets/Models/Farm`; boulders and fragments reuse the imported `Rock_04` mesh with the existing URP farm material. Sprinkler and upgrade components use simple editable mesh children. The watering can and sprinkler streams use a authored repeating texture and scrolling UV offsets (`WaterFlow`), making flow direction visible. The texture and materials are under `Assets/Materials/Farm`.

New prefabs: `Tools/pickaxe`, four `Tools/*Kit` applicators, `Facilities/sprinkler`, `Resources/boulder` and `Resources/looseStone`, all under `Assets/Prefabs/Farm`. `FarmCell` owns cell occupancy; `Sprinkler` owns reservoir and neighbor watering; `RockField` owns bounded random spawning; `PickaxeTool` and `PickaxeHead` own held-tool strike handling; `ActionFeedback` owns popup and sound presentation. No per-frame scene searches are used.

### Updated scoring ledger

Starting tally after crop health: 23. Added: Shatter 2, Wild Instances 2, Intermittent Instances 1, Neighbor Interaction Content 1, Stat Content tier increase 1 (growth, moisture, health, hardness, reservoir water), Resource Content tier increase 1 (nine resources), Crafting Content tier increase 1 (seven recipes), Texture Animation Signifiers 2, and Solar Coaster 1. Feature tally is **35 points before the 33-point individual cap**.

This ledger does not double-count harvesting, caretaking or death, and does not claim separate audio/model-upgrade points. Prefab Content is not included in this tally. First-encounter pop-up tutorials, the final prefab-affordance accounting, headset testing and submission video/contribution documentation remain separate work; existing permanent signs and upgrade feedback are not counted as first-encounter tutorials. Composting and eight-neighbor sprinkler range upgrades are not part of this implementation.

Validation: 53 new mining/building/upgrade/audio/irrigation checks plus 18 tilling/planting, 20 watering/growth, 18 harvesting, 41 trading/resource and 27 health/death checks all passed (177 Play Mode checks). Tests exercised actual held-tool contacts and XR selections, kit affordability/duplicate guards, capacity and retention changes, UV scrolling, bounded respawning, and AudioSource playback. Initial and upgraded scene previews were visually inspected. Audio was muted in the isolated test process; headset loudness, reach and controller ergonomics still require hardware testing.

## Player HUD

The saved scene contains `playerRig/cameraOffset/mainCamera/playerHud`, a world-space Canvas attached to the player camera. Its offset is (0, -0.43, 1.4) metres and scale is 0.0009; adjust these in the Hierarchy for headset comfort. Every panel and label is individually editable, follows lowerCamelCase naming, and has UI raycast targeting disabled.

The HUD immediately shows crops, seeds, coins, fertilizer, stones and all four kit inventories from the existing FarmResources event. Every 0.2 seconds it displays ready crops, dry crops, crops below 50 health, empty sprinkler tanks, and a zero-seed reminder. Looking at a plot within 6 metres reveals its water, growth, crop health, fertilizer/harvest status and soil tier, or a sprinkler's tier and remaining/capacity water. The UI reads existing gameplay state; it does not add player health or a new clock. The market and workshop displays remain available at their stations.

HUD validation: 21 Play Mode checks passed for inventory events, gaze inspection, drought/health/maturity summaries, harvest reset, sprinkler refill, no-seed reminders, re-enable refresh and camera following; the rendered preview was inspected. The 27 crop-health/death/replant checks also passed after adding the read-only FarmCell state accessors. Final headset text comfort remains to be checked in VR.

## Contextual controller prompts

Like MP1B's floating item prompts, authored world-space panels appear while an XR interactor hovers over or selects an object. Nine tools show Hold GRIP (side) to grab; held tools show their specific contact action and Release GRIP to drop. The watering can instead explicitly instructs Hold TRIGGER (index) to pour at soil or a sprinkler tank. Market/workshop buttons show Press GRIP to trade/buy. Mature crops and loose stones inherit harvest/collect prompts from their prefabs; immature crops do not show harvest prompts.

Each target contains editable `interactionPrompt/promptText` children and an `InteractionPrompt` component. Messages and height are Inspector fields. Panels face the player, retain readable scale when tools rotate, and have no colliders or UI raycast targets. They hide when neither hovered nor held. Camera lookup happens once at startup because runtime-spawned prefabs cannot hold a scene-camera reference. No new input bindings or gameplay rules were introduced. These are repeatable contextual control hints; they are not separately claimed as first-encounter-only tutorials.

Validation: 47 Play Mode checks passed for authored references, hidden initial state, nonblocking UI, hover/exit, held-tool instructions, facing/scale, release, market transactions, crop maturity/harvest and spawned fragment prompts. A rendered held-watering-can preview was visually inspected. Final headset readability should be checked at normal hand distance.

## Smooth player turning

The owned playerRig variant now enables smooth turn actions, disables the snap-turn provider and disables turn-around. `LinearTurnProvider` extends the existing XRI continuous-turn provider so yaw depends only on the right stick's horizontal value: input.x * turnSpeed * deltaTime. Vertical stick input never causes a 180-degree turn or changes the horizontal turn rate. The left turn input is unused; left-stick movement remains unchanged. The default maximum speed is 60 degrees/second, editable in the rig's Linear Turn Provider component. Existing input deadzones still apply.

Validation: 14 Play Mode checks passed, including saved action/mediator references, disabled snap turning, half/full input proportionality, diagonal and vertical input, actual rig rotation over time, and stopping after release. No third-party Starter Assets were edited. Headset comfort can be tuned using Turn Speed.
