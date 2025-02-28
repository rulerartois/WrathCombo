using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.PvE;

internal partial class RPR
{
    internal class RPR_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // Don't change anything if not basic skill
            if (actionID is not Slice)
                return actionID;

            //Variant Cure
            if (IsEnabled(CustomComboPreset.RPR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.RPR_VariantCure))
                return Variant.VariantCure;

            //Variant Rampart
            if (IsEnabled(CustomComboPreset.RPR_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave(actionID))
                return Variant.VariantRampart;

            //RPR Opener
            if (RPROpener.DoFullOpener(ref actionID))
                return actionID;

            //All Weaves
            if (CanWeave(ActionWatching.LastWeaponskill))
            {
                //Arcane Cirlce
                if (LevelChecked(ArcaneCircle) &&
                    ((LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) && IsOffCooldown(ArcaneCircle)) ||
                     (!LevelChecked(Enshroud) && IsOffCooldown(ArcaneCircle))))
                    return ArcaneCircle;

                //Enshroud
                if (RPRHelper.UseEnshroud(Gauge))
                    return Enshroud;

                //Gluttony/Bloodstalk
                if (Gauge.Soul >= 50 &&
                    !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                    !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.ImmortalSacrifice) &&
                    !HasEffect(Buffs.IdealHost) && !HasEffect(Buffs.PerfectioParata) &&
                    !RPRHelper.IsComboExpiring(3))
                {
                    //Gluttony
                    if (ActionReady(Gluttony) &&
                        (GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 || !LevelChecked(ArcaneCircle)))
                        return trueNorthReady
                            ? All.TrueNorth
                            : Gluttony;

                    //Bloodstalk
                    if (LevelChecked(BloodStalk) &&
                        (!LevelChecked(Gluttony) ||
                         (LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                          (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 4))))
                        return OriginalHook(BloodStalk);
                }

                //Enshroud Weaves
                if (HasEffect(Buffs.Enshrouded))
                {
                    //Sacrificium
                    if (Gauge.LemureShroud is 2 && GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 &&
                        HasEffect(Buffs.Oblatio) && LevelChecked(Sacrificium))
                        return OriginalHook(Gluttony);

                    //Lemure's Slice
                    if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }
            }

            //Ranged Attacks
            if (!InMeleeRange() && LevelChecked(Harpe) && HasBattleTarget() &&
                !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.SoulReaver))
            {
                //Communio
                if (HasEffect(Buffs.Enshrouded) && Gauge.LemureShroud is 1 &&
                    LevelChecked(Communio))
                    return Communio;

                return HasEffect(Buffs.Soulsow) && LevelChecked(HarvestMoon)
                    ? HarvestMoon
                    : Harpe;
            }

            //Shadow Of Death
            if (RPRHelper.UseShadowOfDeath())
                return ShadowOfDeath;

            if (TargetHasEffect(Debuffs.DeathsDesign))
            {
                //Perfectio
                if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio) && !RPRHelper.IsComboExpiring(1))
                    return OriginalHook(Communio);

                //Gibbet/Gallows
                if (LevelChecked(Gibbet) && !HasEffect(Buffs.Enshrouded) &&
                    (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)))
                {
                    //Gibbet
                    if (HasEffect(Buffs.EnhancedGibbet))
                    {
                        if (trueNorthReady && !OnTargetsFlank() &&
                            CanDelayedWeave(ActionWatching.LastWeaponskill))
                            return All.TrueNorth;

                        return OriginalHook(Gibbet);
                    }

                    //Gallows
                    if (HasEffect(Buffs.EnhancedGallows) ||
                        (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
                    {
                        if (trueNorthReady && !OnTargetsRear() &&
                            CanDelayedWeave(ActionWatching.LastWeaponskill))
                            return All.TrueNorth;

                        return OriginalHook(Gallows);
                    }
                }

                //Plentiful Harvest
                if (LevelChecked(PlentifulHarvest) &&
                    !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                    !HasEffect(Buffs.Executioner) && HasEffect(Buffs.ImmortalSacrifice) &&
                    (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                    return PlentifulHarvest;

                //Enshroud Combo
                if (HasEffect(Buffs.Enshrouded))
                {
                    //Communio
                    if (Gauge.LemureShroud is 1 && LevelChecked(Communio))
                        return Communio;

                    //Void Reaping
                    if (HasEffect(Buffs.EnhancedVoidReaping))
                        return OriginalHook(Gibbet);

                    //Cross Reaping
                    if (HasEffect(Buffs.EnhancedCrossReaping) ||
                        (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping)))
                        return OriginalHook(Gallows);
                }

                //Soul Slice
                if (Gauge.Soul <= 50 && ActionReady(SoulSlice) &&
                    !RPRHelper.IsComboExpiring(3) &&
                    !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                    !HasEffect(Buffs.IdealHost) && !HasEffect(Buffs.Executioner) &&
                    !HasEffect(Buffs.PerfectioParata) && !HasEffect(Buffs.ImmortalSacrifice))
                    return SoulSlice;
            }

            //Healing
            if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                return All.SecondWind;

            if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                return All.Bloodbath;

            //1-2-3 Combo
            if (comboTime > 0)
            {
                if (lastComboMove == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                    return OriginalHook(WaxingSlice);

                if (lastComboMove == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                    return OriginalHook(InfernalSlice);
            }

            return OriginalHook(Slice);
        }
    }

    internal class RPR_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            int PositionalChoice = Config.RPR_Positional;

            // Don't change anything if not basic skill
            if (actionID is not Slice)
                return actionID;

            //Variant Cure
            if (IsEnabled(CustomComboPreset.RPR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.RPR_VariantCure))
                return Variant.VariantCure;

            //Variant Rampart
            if (IsEnabled(CustomComboPreset.RPR_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave(actionID))
                return Variant.VariantRampart;

            //RPR Opener
            if (IsEnabled(CustomComboPreset.RPR_ST_Opener))
                if (RPROpener.DoFullOpener(ref actionID))
                    return actionID;

            //All Weaves
            if (CanWeave(ActionWatching.LastWeaponskill))
            {
                if (IsEnabled(CustomComboPreset.RPR_ST_CDs))
                {
                    //Arcane Cirlce
                    if (IsEnabled(CustomComboPreset.RPR_ST_ArcaneCircle) &&
                        LevelChecked(ArcaneCircle) &&
                        ((LevelChecked(Enshroud) && JustUsed(ShadowOfDeath) && IsOffCooldown(ArcaneCircle)) ||
                         (!LevelChecked(Enshroud) && IsOffCooldown(ArcaneCircle))))
                        return ArcaneCircle;

                    //Enshroud
                    if (IsEnabled(CustomComboPreset.RPR_ST_Enshroud) &&
                        RPRHelper.UseEnshroud(Gauge))
                        return Enshroud;

                    //Gluttony/Bloodstalk
                    if (Gauge.Soul >= 50 &&
                        !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                        !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.ImmortalSacrifice) &&
                        !HasEffect(Buffs.IdealHost) && !HasEffect(Buffs.PerfectioParata) &&
                        !RPRHelper.IsComboExpiring(3))
                    {
                        //Gluttony
                        if (IsEnabled(CustomComboPreset.RPR_ST_Gluttony) &&
                            ActionReady(Gluttony) &&
                            (GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 || !LevelChecked(ArcaneCircle)))
                        {
                            if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                                trueNorthReady)
                                return All.TrueNorth;

                            return Gluttony;
                        }

                        //Bloodstalk
                        if (IsEnabled(CustomComboPreset.RPR_ST_Bloodstalk) &&
                            LevelChecked(BloodStalk) &&
                            (!LevelChecked(Gluttony) ||
                             (LevelChecked(Gluttony) && IsOnCooldown(Gluttony) &&
                              (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) > GCD * 4))))
                            return OriginalHook(BloodStalk);
                    }
                }

                //Enshroud Weaves
                if (HasEffect(Buffs.Enshrouded))
                {
                    //Sacrificium
                    if (IsEnabled(CustomComboPreset.RPR_ST_Sacrificium) &&
                        Gauge.LemureShroud is 2 && GetCooldownRemainingTime(ArcaneCircle) > GCD * 3 &&
                        HasEffect(Buffs.Oblatio) && LevelChecked(Sacrificium))
                        return OriginalHook(Gluttony);

                    //Lemure's Slice
                    if (IsEnabled(CustomComboPreset.RPR_ST_Lemure) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                        return OriginalHook(BloodStalk);
                }
            }

            //Ranged Attacks
            if (IsEnabled(CustomComboPreset.RPR_ST_RangedFiller) &&
                !InMeleeRange() && LevelChecked(Harpe) && HasBattleTarget() &&
                !HasEffect(Buffs.Executioner) && !HasEffect(Buffs.SoulReaver))
            {
                //Communio
                if (HasEffect(Buffs.Enshrouded) && Gauge.LemureShroud is 1 &&
                    LevelChecked(Communio))
                    return Communio;

                return IsEnabled(CustomComboPreset.RPR_ST_RangedFillerHarvestMoon) &&
                       HasEffect(Buffs.Soulsow) && LevelChecked(HarvestMoon)
                    ? HarvestMoon
                    : Harpe;
            }

            //Shadow Of Death
            if (IsEnabled(CustomComboPreset.RPR_ST_SoD) &&
                RPRHelper.UseShadowOfDeath() && GetTargetHPPercent() > Config.RPR_SoDThreshold)
                return ShadowOfDeath;

            if (TargetHasEffect(Debuffs.DeathsDesign))
            {
                //Perfectio
                if (IsEnabled(CustomComboPreset.RPR_ST_Perfectio) &&
                    HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio) && !RPRHelper.IsComboExpiring(1))
                    return OriginalHook(Communio);

                //Gibbet/Gallows
                if (IsEnabled(CustomComboPreset.RPR_ST_GibbetGallows) &&
                    LevelChecked(Gibbet) && !HasEffect(Buffs.Enshrouded) &&
                    (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)))
                {
                    //Gibbet
                    if (HasEffect(Buffs.EnhancedGibbet) ||
                        (PositionalChoice is 1 && !HasEffect(Buffs.EnhancedGibbet) &&
                         !HasEffect(Buffs.EnhancedGallows)))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                            ((IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge) &&
                              GetRemainingCharges(All.TrueNorth) < 2) ||
                             IsNotEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge)) &&
                            trueNorthReady && !OnTargetsFlank() &&
                            CanDelayedWeave(ActionWatching.LastWeaponskill))
                            return All.TrueNorth;

                        return OriginalHook(Gibbet);
                    }

                    //Gallows
                    if (HasEffect(Buffs.EnhancedGallows) ||
                        (PositionalChoice is 0 && !HasEffect(Buffs.EnhancedGibbet) &&
                         !HasEffect(Buffs.EnhancedGallows)))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic) &&
                            ((IsEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge) &&
                              GetRemainingCharges(All.TrueNorth) < 2) ||
                             IsNotEnabled(CustomComboPreset.RPR_ST_TrueNorthDynamic_HoldCharge)) &&
                            trueNorthReady && !OnTargetsRear() &&
                            CanDelayedWeave(ActionWatching.LastWeaponskill))
                            return All.TrueNorth;

                        return OriginalHook(Gallows);
                    }
                }

                //Plentiful Harvest
                if (IsEnabled(CustomComboPreset.RPR_ST_CDs) &&
                    IsEnabled(CustomComboPreset.RPR_ST_PlentifulHarvest) &&
                    LevelChecked(PlentifulHarvest) &&
                    !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                    !HasEffect(Buffs.Executioner) && HasEffect(Buffs.ImmortalSacrifice) &&
                    (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                    return PlentifulHarvest;

                //Enshroud Combo
                if (HasEffect(Buffs.Enshrouded))
                {
                    //Communio
                    if (IsEnabled(CustomComboPreset.RPR_ST_Communio) &&
                        Gauge.LemureShroud is 1 && LevelChecked(Communio))
                        return Communio;

                    //Void Reaping
                    if (IsEnabled(CustomComboPreset.RPR_ST_Reaping) &&
                        HasEffect(Buffs.EnhancedVoidReaping))
                        return OriginalHook(Gibbet);

                    //Cross Reaping
                    if (IsEnabled(CustomComboPreset.RPR_ST_Reaping) &&
                        (HasEffect(Buffs.EnhancedCrossReaping) ||
                         (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping))))
                        return OriginalHook(Gallows);
                }

                //Soul Slice
                if (IsEnabled(CustomComboPreset.RPR_ST_SoulSlice) &&
                    Gauge.Soul <= 50 && ActionReady(SoulSlice) &&
                    !RPRHelper.IsComboExpiring(3) &&
                    !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) &&
                    !HasEffect(Buffs.IdealHost) && !HasEffect(Buffs.Executioner) &&
                    !HasEffect(Buffs.PerfectioParata) && !HasEffect(Buffs.ImmortalSacrifice))
                    return SoulSlice;
            }

            //Healing
            if (IsEnabled(CustomComboPreset.RPR_ST_ComboHeals))
            {
                if (PlayerHealthPercentageHp() <= Config.RPR_STSecondWindThreshold && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (PlayerHealthPercentageHp() <= Config.RPR_STBloodbathThreshold && ActionReady(All.Bloodbath))
                    return All.Bloodbath;
            }

            //1-2-3 Combo
            if (comboTime > 0)
            {
                if (lastComboMove == OriginalHook(Slice) && LevelChecked(WaxingSlice))
                    return OriginalHook(WaxingSlice);

                if (lastComboMove == OriginalHook(WaxingSlice) && LevelChecked(InfernalSlice))
                    return OriginalHook(InfernalSlice);
            }

            return OriginalHook(Slice);
        }
    }

    internal class RPR_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // Don't change anything if not basic skill
            if (actionID is not SpinningScythe)
                return actionID;

            if (IsEnabled(CustomComboPreset.RPR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.RPR_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.RPR_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave(actionID))
                return Variant.VariantRampart;

            if (LevelChecked(WhorlOfDeath) &&
                GetDebuffRemainingTime(Debuffs.DeathsDesign) < 6 && !HasEffect(Buffs.SoulReaver) &&
                !HasEffect(Buffs.Executioner))
                return WhorlOfDeath;

            if (TargetHasEffect(Debuffs.DeathsDesign))
            {
                if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                    return OriginalHook(Communio);

                if (HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest) &&
                    !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.Executioner) &&
                    (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                    return PlentifulHarvest;

                if (CanWeave(actionID))
                {
                    if (LevelChecked(ArcaneCircle) &&
                        (GetCooldownRemainingTime(ArcaneCircle) <= GCD + 0.25 || ActionReady(ArcaneCircle)))
                        return ArcaneCircle;

                    if (!HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                        !HasEffect(Buffs.Executioner) &&
                        ActionReady(Enshroud) && (Gauge.Shroud >= 50 || HasEffect(Buffs.IdealHost)) &&
                        !RPRHelper.IsComboExpiring(6))
                        return Enshroud;

                    if (LevelChecked(Gluttony) && Gauge.Soul >= 50 && !HasEffect(Buffs.Enshrouded) &&
                        !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                        (GetCooldownRemainingTime(Gluttony) <= GetCooldownRemainingTime(Slice) + 0.25 ||
                         ActionReady(Gluttony)))
                        return Gluttony;

                    if (LevelChecked(GrimSwathe) && !HasEffect(Buffs.Enshrouded) &&
                        !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                        !HasEffect(Buffs.Executioner) &&
                        Gauge.Soul >= 50 && (!LevelChecked(Gluttony) || (LevelChecked(Gluttony) &&
                                                                         (Gauge.Soul is 100 ||
                                                                          GetCooldownRemainingTime(Gluttony) >
                                                                          GCD * 5))))
                        return GrimSwathe;
                }

                if (!HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner) &&
                    !HasEffect(Buffs.PerfectioParata) &&
                    ActionReady(SoulScythe) && Gauge.Soul <= 50)
                    return SoulScythe;

                if (HasEffect(Buffs.Enshrouded))
                {
                    if (Gauge.LemureShroud is 1 && Gauge.VoidShroud is 0 && ActionReady(Communio))
                        return Communio;

                    if (Gauge.LemureShroud is 2 && Gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                        return OriginalHook(GrimSwathe);

                    if (Gauge.LemureShroud > 0)
                        return OriginalHook(Guillotine);
                }
            }

            if (HasEffect(Buffs.SoulReaver) || (HasEffect(Buffs.Executioner)
                                                && !HasEffect(Buffs.Enshrouded) && LevelChecked(Guillotine)))
                return OriginalHook(Guillotine);

            return lastComboMove == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                ? OriginalHook(NightmareScythe)
                : OriginalHook(SpinningScythe);
        }
    }

    internal class RPR_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            // Don't change anything if not basic skill
            if (actionID is not SpinningScythe)
                return actionID;

            if (IsEnabled(CustomComboPreset.RPR_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= GetOptionValue(Config.RPR_VariantCure))
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.RPR_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanWeave(actionID))
                return Variant.VariantRampart;

            if (IsEnabled(CustomComboPreset.RPR_AoE_WoD) &&
                LevelChecked(WhorlOfDeath) &&
                GetDebuffRemainingTime(Debuffs.DeathsDesign) < 6 && !HasEffect(Buffs.SoulReaver) &&
                GetTargetHPPercent() > Config.RPR_WoDThreshold)
                return WhorlOfDeath;

            if (TargetHasEffect(Debuffs.DeathsDesign))
            {
                if (IsEnabled(CustomComboPreset.RPR_AoE_Perfectio) &&
                    HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                    return OriginalHook(Communio);

                if (IsEnabled(CustomComboPreset.RPR_AoE_CDs))
                {
                    if (IsEnabled(CustomComboPreset.RPR_AoE_PlentifulHarvest) &&
                        HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest) &&
                        !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                        (GetBuffRemainingTime(Buffs.BloodsownCircle) <= 1 || JustUsed(Communio)))
                        return PlentifulHarvest;

                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.RPR_AoE_ArcaneCircle) &&
                            LevelChecked(ArcaneCircle) &&
                            (GetCooldownRemainingTime(ArcaneCircle) <= GCD + 0.25 || ActionReady(ArcaneCircle)))
                            return ArcaneCircle;

                        if (IsEnabled(CustomComboPreset.RPR_AoE_Enshroud) &&
                            !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Enshrouded) &&
                            ActionReady(Enshroud) && (Gauge.Shroud >= 50 || HasEffect(Buffs.IdealHost)) &&
                            !RPRHelper.IsComboExpiring(6))
                            return Enshroud;

                        if (IsEnabled(CustomComboPreset.RPR_AoE_Gluttony) &&
                            LevelChecked(Gluttony) && Gauge.Soul >= 50 && !HasEffect(Buffs.Enshrouded) &&
                            !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                            (GetCooldownRemainingTime(Gluttony) <= GetCooldownRemainingTime(Slice) + 0.25 ||
                             ActionReady(Gluttony)))
                            return Gluttony;

                        if (IsEnabled(CustomComboPreset.RPR_AoE_GrimSwathe) &&
                            LevelChecked(GrimSwathe) && !HasEffect(Buffs.Enshrouded) &&
                            !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.ImmortalSacrifice) &&
                            Gauge.Soul >= 50 &&
                            (!LevelChecked(Gluttony) || (LevelChecked(Gluttony) &&
                                                         (Gauge.Soul is 100 || GetCooldownRemainingTime(Gluttony) >
                                                             GCD * 5))))
                            return GrimSwathe;
                    }
                }

                if (IsEnabled(CustomComboPreset.RPR_AoE_SoulScythe) &&
                    !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver) && !HasEffect(Buffs.Executioner) &&
                    !HasEffect(Buffs.PerfectioParata) &&
                    ActionReady(SoulScythe) && Gauge.Soul <= 50)
                    return SoulScythe;

                if (HasEffect(Buffs.Enshrouded))
                {
                    if (IsEnabled(CustomComboPreset.RPR_AoE_Communio) &&
                        Gauge.LemureShroud is 1 && Gauge.VoidShroud is 0 && ActionReady(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_AoE_Sacrificium) &&
                        Gauge.LemureShroud is 2 && Gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio) &&
                        CanWeave(actionID))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(CustomComboPreset.RPR_AoE_Lemure) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                        return OriginalHook(GrimSwathe);

                    if (IsEnabled(CustomComboPreset.RPR_AoE_Reaping) &&
                        Gauge.LemureShroud > 0)
                        return OriginalHook(Guillotine);
                }
            }

            if (IsEnabled(CustomComboPreset.RPR_AoE_ComboHeals))
            {
                if (PlayerHealthPercentageHp() <= Config.RPR_AoESecondWindThreshold && ActionReady(All.SecondWind))
                    return All.SecondWind;

                if (PlayerHealthPercentageHp() <= Config.RPR_AoEBloodbathThreshold && ActionReady(All.Bloodbath))
                    return All.Bloodbath;
            }

            if (IsEnabled(CustomComboPreset.RPR_AoE_Guillotine) &&
                (HasEffect(Buffs.SoulReaver) || (HasEffect(Buffs.Executioner)
                                                 && !HasEffect(Buffs.Enshrouded) && LevelChecked(Guillotine))))
                return OriginalHook(Guillotine);

            return lastComboMove == OriginalHook(SpinningScythe) && LevelChecked(NightmareScythe)
                ? OriginalHook(NightmareScythe)
                : OriginalHook(SpinningScythe);
        }
    }

    internal class RPR_GluttonyBloodSwathe : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_GluttonyBloodSwathe;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            switch (actionID)
            {
                case GrimSwathe:
                {
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            switch (Gauge.LemureShroud)
                            {
                                case 1 when Gauge.VoidShroud == 0 && LevelChecked(Communio):
                                    return Communio;

                                case 2 when Gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio):
                                    return OriginalHook(Gluttony);
                            }

                            if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe))
                                return OriginalHook(GrimSwathe);

                            if (Gauge.LemureShroud > 1)
                                return OriginalHook(Guillotine);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                        HasEffect(Buffs.Enshrouded) && HasEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)) && LevelChecked(Guillotine))
                        return Guillotine;

                    break;
                }

                case BloodStalk when IsEnabled(CustomComboPreset.RPR_TrueNorthGluttony) &&
                                     trueNorthReady:
                    return All.TrueNorth;

                case BloodStalk:
                {
                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Enshroud))
                    {
                        if (HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio))
                            return OriginalHook(Communio);

                        if (HasEffect(Buffs.Enshrouded))
                        {
                            switch (Gauge.LemureShroud)
                            {
                                case 1 when Gauge.VoidShroud == 0 && LevelChecked(Communio):
                                    return Communio;

                                case 2 when Gauge.VoidShroud is 1 && HasEffect(Buffs.Oblatio):
                                    return OriginalHook(Gluttony);
                            }

                            if (Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice))
                                return OriginalHook(BloodStalk);

                            if (HasEffect(Buffs.EnhancedVoidReaping))
                                return OriginalHook(Gibbet);

                            if (HasEffect(Buffs.EnhancedCrossReaping) ||
                                (!HasEffect(Buffs.EnhancedCrossReaping) && !HasEffect(Buffs.EnhancedVoidReaping)))
                                return OriginalHook(Gallows);
                        }
                    }

                    if (ActionReady(Gluttony) && !HasEffect(Buffs.Enshrouded) && !HasEffect(Buffs.SoulReaver))
                        return Gluttony;

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_Sacrificium) &&
                        HasEffect(Buffs.Enshrouded) && HasEffect(Buffs.Oblatio))
                        return OriginalHook(Gluttony);

                    if (IsEnabled(CustomComboPreset.RPR_GluttonyBloodSwathe_BloodSwatheCombo) &&
                        (HasEffect(Buffs.SoulReaver) || HasEffect(Buffs.Executioner)) && LevelChecked(Gibbet))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedGallows) ||
                            (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
                            return OriginalHook(Gallows);
                    }

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_ArcaneCirclePlentifulHarvest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } =
            CustomComboPreset.RPR_ArcaneCirclePlentifulHarvest;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is ArcaneCircle &&
                   HasEffect(Buffs.ImmortalSacrifice) && LevelChecked(PlentifulHarvest)
                ? PlentifulHarvest
                : actionID;
        }
    }

    internal class RPR_Regress : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Regress;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            return actionID is HellsEgress or HellsIngress && FindEffect(Buffs.Threshold)?.RemainingTime <= 9
                ? Regress
                : actionID;
        }
    }

    internal class RPR_Soulsow : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_Soulsow;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            bool[] soulSowOptions = Config.RPR_SoulsowOptions;
            bool soulsowReady = LevelChecked(Soulsow) && !HasEffect(Buffs.Soulsow);

            return (soulSowOptions.Length > 0 && ((actionID is Harpe && soulSowOptions[0]) ||
                                                  (actionID is Slice && soulSowOptions[1]) ||
                                                  (actionID is SpinningScythe && soulSowOptions[2]) ||
                                                  (actionID is ShadowOfDeath && soulSowOptions[3]) ||
                                                  (actionID is BloodStalk && soulSowOptions[4])) && soulsowReady &&
                    !InCombat()) ||
                   (IsEnabled(CustomComboPreset.RPR_Soulsow_Combat) && actionID is Harpe && !HasBattleTarget())
                ? Soulsow
                : actionID;
        }
    }

    internal class RPR_EnshroudProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudProtection;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            switch (actionID)
            {
                case Enshroud when IsEnabled(CustomComboPreset.RPR_TrueNorthEnshroud) &&
                                   GetBuffStacks(Buffs.SoulReaver) is 2 && trueNorthReady && CanDelayedWeave(Slice):
                    return All.TrueNorth;

                case Enshroud:
                {
                    if (HasEffect(Buffs.SoulReaver))
                    {
                        if (HasEffect(Buffs.EnhancedGibbet))
                            return OriginalHook(Gibbet);

                        if (HasEffect(Buffs.EnhancedGallows) ||
                            (!HasEffect(Buffs.EnhancedGibbet) && !HasEffect(Buffs.EnhancedGallows)))
                            return OriginalHook(Gallows);
                    }

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_CommunioOnGGG : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_CommunioOnGGG;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            switch (actionID)
            {
                case Gibbet or Gallows when HasEffect(Buffs.Enshrouded):
                {
                    if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresSlice) && CanWeave(actionID))
                        return OriginalHook(BloodStalk);

                    break;
                }

                case Guillotine when HasEffect(Buffs.Enshrouded):
                {
                    if (Gauge is { LemureShroud: 1, VoidShroud: 0 } && LevelChecked(Communio))
                        return Communio;

                    if (IsEnabled(CustomComboPreset.RPR_LemureOnGGG) &&
                        Gauge.VoidShroud >= 2 && LevelChecked(LemuresScythe) && CanWeave(actionID))
                        return OriginalHook(GrimSwathe);

                    break;
                }
            }

            return actionID;
        }
    }

    internal class RPR_EnshroudCommunio : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RPR_EnshroudCommunio;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            switch (actionID)
            {
                case Enshroud when HasEffect(Buffs.PerfectioParata) && LevelChecked(Perfectio):
                    return OriginalHook(Communio);

                case Enshroud when HasEffect(Buffs.Enshrouded) && LevelChecked(Communio):
                    return Communio;

                default:
                    return actionID;
            }
        }
    }

    #region ID's

    public const byte JobID = 39;

    public const uint

        // Single Target
        Slice = 24373,
        WaxingSlice = 24374,
        InfernalSlice = 24375,
        ShadowOfDeath = 24378,
        SoulSlice = 24380,

        // AoE
        SpinningScythe = 24376,
        NightmareScythe = 24377,
        WhorlOfDeath = 24379,
        SoulScythe = 24381,

        // Unveiled
        Gibbet = 24382,
        Gallows = 24383,
        Guillotine = 24384,
        UnveiledGibbet = 24390,
        UnveiledGallows = 24391,
        ExecutionersGibbet = 36970,
        ExecutionersGallows = 36971,
        ExecutionersGuillotine = 36972,

        // Reaver
        BloodStalk = 24389,
        GrimSwathe = 24392,
        Gluttony = 24393,

        // Sacrifice
        ArcaneCircle = 24405,
        PlentifulHarvest = 24385,

        // Enshroud
        Enshroud = 24394,
        Communio = 24398,
        LemuresSlice = 24399,
        LemuresScythe = 24400,
        VoidReaping = 24395,
        CrossReaping = 24396,
        GrimReaping = 24397,
        Sacrificium = 36969,
        Perfectio = 36973,

        // Miscellaneous
        HellsIngress = 24401,
        HellsEgress = 24402,
        Regress = 24403,
        Harpe = 24386,
        Soulsow = 24387,
        HarvestMoon = 24388;

    public static class Buffs
    {
        public const ushort
            SoulReaver = 2587,
            ImmortalSacrifice = 2592,
            ArcaneCircle = 2599,
            EnhancedGibbet = 2588,
            EnhancedGallows = 2589,
            EnhancedVoidReaping = 2590,
            EnhancedCrossReaping = 2591,
            EnhancedHarpe = 2845,
            Enshrouded = 2593,
            Soulsow = 2594,
            Threshold = 2595,
            BloodsownCircle = 2972,
            IdealHost = 3905,
            Oblatio = 3857,
            Executioner = 3858,
            PerfectioParata = 3860;
    }

    public static class Debuffs
    {
        public const ushort
            DeathsDesign = 2586;
    }

    #endregion
}