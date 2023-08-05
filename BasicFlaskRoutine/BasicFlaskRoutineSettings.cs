using System.Windows.Forms;
using ExileCore.Shared.Nodes;
using TreeRoutine.Routine.BasicFlaskRoutine.Flask;

namespace TreeRoutine.Routine.BasicFlaskRoutine;

public class BasicFlaskRoutineSettings : BaseTreeSettings
{
    public RangeNode<int> TicksPerSecond { get; set; } = new(10, 1, 30);

    public ToggleNode EnableInHideout { get; set; } = new(false);

    public ToggleNode AutoFlask { get; set; } = new(false);

    public ToggleNode ForceBubblingAsInstantOnly { get; set; } = new(false);
    public ToggleNode ForcePanickedAsInstantOnly { get; set; } = new(false);

    public RangeNode<int> HPPotion { get; set; } = new(65, 0, 100);
    public RangeNode<int> InstantHPPotion { get; set; } = new(50, 0, 100);
    public ToggleNode AllocatedSupremeDecadence { get; set; } = new(false);
    public RangeNode<int> ESPotion { get; set; } = new(65, 0, 100);
    public RangeNode<int> InstantESPotion { get; set; } = new(50, 0, 100);
    public ToggleNode DisableLifeSecUse { get; set; } = new(false);

    public RangeNode<int> ManaPotion { get; set; } = new(65, 0, 100);
    public RangeNode<int> InstantManaPotion { get; set; } = new(35, 0, 100);
    public RangeNode<int> MinManaFlask { get; set; } = new(50, 0, 100);


    public ToggleNode RemAilment { get; set; } = new(false);
    public ToggleNode RemFrozen { get; set; } = new(false);
    public ToggleNode RemBurning { get; set; } = new(false);
    public ToggleNode RemShocked { get; set; } = new(false);
    public ToggleNode RemCurse { get; set; } = new(false);
    public ToggleNode RemPoison { get; set; } = new(false);
    public ToggleNode RemBleed { get; set; } = new(false);
    public RangeNode<int> CorruptCount { get; set; } = new(10, 0, 20);


    public ToggleNode SpeedFlaskEnable { get; set; } = new(false);
    public ToggleNode QuicksilverFlaskEnable { get; set; } = new(false);
    public ToggleNode SilverFlaskEnable { get; set; } = new(false);
    public RangeNode<int> MinMsPlayerMoving { get; set; } = new(1500, 1, 10000);

    public ToggleNode UseWhileCycloning { get; set; } = new(false);
    public RangeNode<int> CycloningMonsterCount { get; set; } = new(0, 0, 30);
    public RangeNode<int> CycloningMonsterDistance { get; set; } = new(400, 0, 1500);
    public ToggleNode CycloningCountNormalMonsters { get; set; } = new(false);
    public ToggleNode CycloningCountMagicMonsters { get; set; } = new(false);
    public ToggleNode CycloningCountRareMonsters { get; set; } = new(false);
    public ToggleNode CycloningCountUniqueMonsters { get; set; } = new(false);
    public ToggleNode CycloningIgnoreFullHealthUniqueMonsters { get; set; } = new(false);


    public ToggleNode DefensiveFlaskEnable { get; set; } = new(false);
    public RangeNode<int> HPPercentDefensive { get; set; } = new(50, 0, 100);
    public RangeNode<int> ESPercentDefensive { get; set; } = new(50, 0, 100);
    public ToggleNode OffensiveAsDefensiveEnable { get; set; } = new(false);

    public RangeNode<int> DefensiveMonsterCount { get; set; } = new(0, 0, 30);
    public RangeNode<int> DefensiveMonsterDistance { get; set; } = new(400, 0, 1500);
    public ToggleNode DefensiveCountNormalMonsters { get; set; } = new(false);
    public ToggleNode DefensiveCountRareMonsters { get; set; } = new(false);
    public ToggleNode DefensiveCountMagicMonsters { get; set; } = new(false);
    public ToggleNode DefensiveCountUniqueMonsters { get; set; } = new(false);
    public ToggleNode DefensiveIgnoreFullHealthUniqueMonsters { get; set; } = new(false);
    public ToggleNode BossingModeToggle { get; set; } = new(false);
    public bool BossingMode = false;
    public HotkeyNode BossingModeHotkey { get; set; } = new(Keys.T);


    public ToggleNode OffensiveFlaskEnable { get; set; } = new(false);
    public RangeNode<int> HPPercentOffensive { get; set; } = new(50, 0, 100);
    public RangeNode<int> ESPercentOffensive { get; set; } = new(50, 0, 100);

    public RangeNode<int> OffensiveMonsterCount { get; set; } = new(0, 0, 30);
    public RangeNode<int> OffensiveMonsterDistance { get; set; } = new(400, 0, 1500);
    public ToggleNode OffensiveCountNormalMonsters { get; set; } = new(false);
    public ToggleNode OffensiveCountRareMonsters { get; set; } = new(false);
    public ToggleNode OffensiveCountMagicMonsters { get; set; } = new(false);
    public ToggleNode OffensiveCountUniqueMonsters { get; set; } = new(false);
    public ToggleNode OffensiveIgnoreFullHealthUniqueMonsters { get; set; } = new(false);


    public FlaskSetting[] FlaskSettings { get; set; } =
    {
        new FlaskSetting(new ToggleNode(true), new HotkeyNode(Keys.D1), new RangeNode<int>(0, 0, 5)),
        new FlaskSetting(new ToggleNode(true), new HotkeyNode(Keys.D2), new RangeNode<int>(0, 0, 5)),
        new FlaskSetting(new ToggleNode(true), new HotkeyNode(Keys.D3), new RangeNode<int>(0, 0, 5)),
        new FlaskSetting(new ToggleNode(true), new HotkeyNode(Keys.D4), new RangeNode<int>(0, 0, 5)),
        new FlaskSetting(new ToggleNode(true), new HotkeyNode(Keys.D5), new RangeNode<int>(0, 0, 5))
    };
}