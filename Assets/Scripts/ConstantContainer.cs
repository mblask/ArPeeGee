using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType
{
    Default,
    Interactable
}

public enum FountainType
{
    Health,
    Magic
}

public enum PotionType
{
    Health,
    Magic
}

public enum EquipmentSlot
{
    Armor,
    Weapon,
    Ring,
}

public enum Essence
{
    Health,
    Magic
}

public static class ConstantContainer
{
    public const string BackgroundLayerString = "Background";
    public const string MainMenuName = "MainMenu";

    public const string PlayerWizardText = "PlayerWizard(Clone)";
    public const string PlayerKnightText = "PlayerKnight(Clone)";
    public const string WizardText = "Wizard";
    public const string KnightText = "Knight";

    public static Color RegularSpriteColor = new Color(255.0f, 255.0f, 255.0f, 255.0f);
    public static Color StrongTransparency = new Color(255.0f, 255.0f, 255.0f, 0.15f);
}
