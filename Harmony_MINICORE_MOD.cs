using System;
using System.Reflection;
using Harmony;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

namespace NULL_MINICORE_MOD
{
    public class Harmony_Patch
    {
        public Harmony_Patch()
        {
            try
            {
                HarmonyInstance harmonyInstance = HarmonyInstance.Create("Lobotomy.NULL.MINICORE");

                harmonyInstance.Patch(typeof(CreatureManager).GetMethod("OnFixedUpdate"), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("OnFixedUpdate_Creature")), null, null);
                harmonyInstance.Patch(typeof(UseSkill).GetMethod("InitUseSkillAction", AccessTools.all), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("InitUseSkillAction", AccessTools.all)), null);
                harmonyInstance.Patch(typeof(GameManager).GetMethod("ClearStage", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ClearStage_patch", AccessTools.all)), null, null);

                harmonyInstance.Patch(typeof(BgmManager).GetMethod("ResetBgm", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ResetBgm_patch", AccessTools.all)), null, null);
                harmonyInstance.Patch(typeof(BgmManager).GetMethod("SetBgm", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("SetBgm_patch", AccessTools.all)), null, null);

                harmonyInstance.Patch(typeof(CreatureOverloadManager).GetMethod("ActivateOverload"), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ActivateOverload")), null, null);
                harmonyInstance.Patch(typeof(GameStatusUI.EnergyController).GetMethod("SetOverloadIsolateNum"), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("SetOverloadIsolateNum")), null, null);
                harmonyInstance.Patch(typeof(CreatureOverloadManager).GetMethod("AddOverloadGague"), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("AddOverloadGague")), null, null);

                harmonyInstance.Patch(typeof(AgentModel).GetMethod("GetMovementValue", AccessTools.all), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("GetMovementValue_patch", AccessTools.all)), null);

                harmonyInstance.Patch(typeof(EquipmentModel).GetMethod("OnTakeDamage", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("OnTakeDamage_Patch", AccessTools.all)), null, null);
                harmonyInstance.Patch(typeof(WorkerModel).GetMethod("TakeDamageWithoutEffect", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("TakeDamageWithoutEffect_Patch", AccessTools.all)), null, null);

                harmonyInstance.Patch(typeof(UseSkill).GetMethod("CheckLive", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("CheckLive_patch", AccessTools.all)), null, null);
                harmonyInstance.Patch(typeof(CreatureModel).GetMethod("GetRiskLevel", AccessTools.all), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("GetRiskLevel_patch", AccessTools.all)), null);
            } catch(Exception exp)
            {

            }
        }

        public static bool ResetBgm_patch()
        {
            int time = (int)GlobalHistory.instance.GetCurrentTime();
            if (CoreCh && time > 0)
            {
                return false;
            }
            return true;
        }

        public static bool SetBgm_patch()
        {
            int time = (int)GlobalHistory.instance.GetCurrentTime();
            if (CoreCh && time > 0)
            {
                return false;
            }
            return true;
        }

        public static bool ActivateOverload(ref int overloadCount, ref OverloadType type, ref float overloadTime, ref bool ignoreWork , ref bool ignoreBossReward , ref bool ignoreDefaultOverload)
        {
            int time = (int)GlobalHistory.instance.GetCurrentTime();
            if (CoreCh && time > 0)
            {
                int currentDay = PlayerModel.instance.GetDay() + 1;
                int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
                overloadCount = (int)(currentDay / 4 * (1 + 0.25 * level));
                ignoreBossReward = true;

                if (ordealCoreCh)
                    overloadCount = 0;
            }

            return true;
        }

        public static bool SetOverloadIsolateNum(ref int num)
        {
            int time = (int)GlobalHistory.instance.GetCurrentTime();
            if (CoreCh && time > 0)
            {
                int currentDay = PlayerModel.instance.GetDay() + 1;
                int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
                num = (int)(currentDay/4 * (1+0.25*level));

                if (ordealCoreCh)
                    num = 0;
            }
            return true;
        }

        public static bool AddOverloadGague(CreatureOverloadManager __instance)
        {
            if(ordealCoreCh)
            {
                __instance.GetType().GetField("_nextOrdeal", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, null);
            }
            return true;
        }

        public static bool OnFixedUpdate_Creature()
        {
            int time = (int)GlobalHistory.instance.GetCurrentTime();
            if (time == 0)
            {
                CoreCh = false;

                speedCh = false;
                damageTypeCh = false;
                peCheckCoreCh = false;
                fearCoreCh = false;
                ordealCoreCh = false;

                prNum = 0;
                LoadData();
            }

            if(CoreCh)
            {
                if(speedCh)
                {
                    if(CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 2 && prNum == 0)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/1_Tilarids - Violation Of Black Colors"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 3 && prNum == 1)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/1_Tilarids - Violation Of Black Colors"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 4 && prNum == 2)
                    { 
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 5 && prNum == 3)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 6 && prNum == 4)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 7 && prNum == 5)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 8 && prNum == 6)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 9 && prNum == 7)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 10 && prNum == 8)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Malkuth/2_Tilarids - Red Dots"));
                    }
                }
                else if(damageTypeCh)
                {
                    if(CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 2 && prNum == 0)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/1_3 - Abandoned"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 3 && prNum == 1)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/1_3 - Abandoned"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 4 && prNum == 2)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/1_3 - Abandoned"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 5 && prNum == 3)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/1_3 - Abandoned"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 6 && prNum == 4)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/2_Tilarids - Blue Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 7 && prNum == 5)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/2_Tilarids - Blue Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 8 && prNum == 6)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/2_Tilarids - Blue Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 9 && prNum == 7)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/2_Tilarids - Blue Dots"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 10 && prNum == 8)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Netzach/2_Tilarids - Blue Dots"));
                    }
                }
                else if (peCheckCoreCh)
                {
                    if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 2 && prNum == 0)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/1_Theme_-_Retro_Time_ALT"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 3 && prNum == 1)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/1_Theme_-_Retro_Time_ALT"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 4 && prNum == 2)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 5 && prNum == 3)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 6 && prNum == 4)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 7 && prNum == 5)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 8 && prNum == 6)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 9 && prNum == 7)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 10 && prNum == 8)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Hod/2_Theme_-_Retro_Time_ALT(Mix)"));
                    }
                }
                else if (fearCoreCh)
                {
                    if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 2 && prNum == 0)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/1_Tilarids - untitled9877645623413123325"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 3 && prNum == 1)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/1_Tilarids - untitled9877645623413123325"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 4 && prNum == 2)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 5 && prNum == 3)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 6 && prNum == 4)
                    {
                        malkutBase.OnChangePhase(); prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 7 && prNum == 5)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 8 && prNum == 6)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 9 && prNum == 7)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 10 && prNum == 8)
                    {
                        prNum++; playBGM(Resources.Load<AudioClip>("Sounds/BGM/Boss/Yesod/2_Tilarids - Faded"));
                    }
                }
                else if (ordealCoreCh)
                {
                    if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 2 && prNum == 0)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new MachineDawnOrdeal().OnOrdealStart();
                        new BugDawnOrdeal().OnOrdealStart();
                        new OutterGodDawnOrdeal().OnOrdealStart();
                        new CircusDawnOrdeal().OnOrdealStart();
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 3 && prNum == 1)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new MachineNoonOrdeal().OnOrdealStart();
                        new CircusNoonOrdeal().OnOrdealStart();
                        new ScavengerOrdeal().OnOrdealStart();

                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 4 && prNum == 2)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new MachineDuskOrdeal().OnOrdealStart();
                        new CircusDuskOrdeal().OnOrdealStart();

                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 5 && prNum == 3)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new MachineMidnightOrdeal().OnOrdealStart();
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 6 && prNum == 4)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new OutterGodMidnightOrdeal().OnOrdealStart();
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 7 && prNum == 5)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new BugMidnightOrdeal().OnOrdealStart();
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 8 && prNum == 6)
                    {
                        netzachBase.OnOverloadActivated(0); prNum++; loadUndertailMusic();
                        new MachineMidnightOrdeal().OnOrdealStart();
                        new OutterGodMidnightOrdeal().OnOrdealStart();
                        new BugMidnightOrdeal().OnOrdealStart();
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 9 && prNum == 7)
                    {
                        prNum++; loadUndertailMusic();
                    }
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() == 10 && prNum == 8)
                    {
                        prNum++; loadUndertailMusic();
                    }
                }
            }

            if (isOpen && !CoreCh)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    closeDesc();
                    if (speedCoreCleared)
                        openDesc("! 이미 억제가 완료되었습니다 !", 500, 250, 1500, 500);
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() != 1)
                            openDesc("! 클리포트 폭주 레벨이 1이 아닙니다 !", 500, 250, 1500, 500);
                    else if (checkToday(25))
                    {
                        speedCh = true;
                        CoreCh = true;
                        malkutBase = new MalkutBossBase();
                        malkutBase.OnStageStart();
                        playBGM(BgmManager.instance.emergencyLevel_2.GetRandomClip());
                    }
                    else
                        openDesc("! 조건이 충족되지 않습니다. !", 500, 250, 1500, 500);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    closeDesc();
                    if (damageTypeCoreCleared)
                        openDesc("! 이미 억제가 완료되었습니다 !", 500, 250, 1500, 500);
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() != 1)
                        openDesc("! 클리포트 폭주 레벨이 1이 아닙니다 !", 500, 250, 1500, 500);
                    else if (checkToday(30))
                    {
                        damageTypeCh = true;
                        CoreCh = true;
                        netzachBase = new NetzachBossBase();
                        netzachBase.OnStageStart();
                        playBGM(BgmManager.instance.emergencyLevel_2.GetRandomClip());
                    }
                    else
                        openDesc("! 조건이 충족되지 않습니다. !", 500, 250, 1500, 500);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    closeDesc();
                    if (peCheckCoreCleared)
                        openDesc("! 이미 억제가 완료되었습니다 !", 500, 250, 1500, 500);
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() != 1)
                        openDesc("! 클리포트 폭주 레벨이 1이 아닙니다 !", 500, 250, 1500, 500);
                    else if (checkToday(30))
                    {
                        peCheckCoreCh = true;
                        CoreCh = true;
                        malkutBase = new MalkutBossBase();
                        malkutBase.OnStageStart();
                        playBGM(BgmManager.instance.emergencyLevel_2.GetRandomClip());
                    }
                    else
                        openDesc("! 조건이 충족되지 않습니다. !", 500, 250, 1500, 500);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    closeDesc();
                    if (fearCoreCleared)
                        openDesc("! 이미 억제가 완료되었습니다 !", 500, 250, 1500, 500);
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() != 1)
                        openDesc("! 클리포트 폭주 레벨이 1이 아닙니다 !", 500, 250, 1500, 500);
                    else if (checkToday(30))
                    {
                        fearCoreCh = true;
                        CoreCh = true;
                        malkutBase = new MalkutBossBase();
                        malkutBase.OnStageStart();
                        playBGM(BgmManager.instance.emergencyLevel_2.GetRandomClip());
                    }
                    else
                        openDesc("! 조건이 충족되지 않습니다. !", 500, 250, 1500, 500);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    closeDesc();
                    if (ordealCoreCleared)
                        openDesc("! 이미 억제가 완료되었습니다 !", 500, 250, 1500, 500);
                    else if (CreatureOverloadManager.instance.GetQliphothOverloadLevel() != 1)
                        openDesc("! 클리포트 폭주 레벨이 1이 아닙니다 !", 500, 250, 1500, 500);
                    else if (checkToday(35) && speedCoreCleared && damageTypeCoreCleared && peCheckCoreCleared && fearCoreCleared)
                    {
                        ordealCoreCh = true;
                        CoreCh = true;
                        netzachBase = new NetzachBossBase();
                        netzachBase.OnStageStart();
                        playBGM(BgmManager.instance.emergencyLevel_3.GetRandomClip());
                    }
                    else
                        openDesc("! 조건이 충족되지 않습니다. !", 500, 250, 1500, 500);
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!File.Exists(filePath))
                {
                    SaveData();
                }
                else
                {
                    LoadData();
                }

                if (!isOpen)
                {
                    String desc = "* 미니 코어 억제 목록 (목록이 보이는 상태에서 제목 옆의 문자에 해당하는 키를 누르면 활성화됨) ('p'로 닫기)" +
                        "\n1. 스피드 코어 억제[1] - 직원들의 속도에 이상이 감지됩니다." +
                        ((speedCoreCleared) ? "<억제 완료>" : "<억제 미완료>") +
                        "\n- 보상 : 모든 직원들의 작업속도 25% 증가" +
                        "\n- 활성 조건 : Day 25 이상" +
                        "\n- 목표 : 클리포트 카운터 7 달성 및 에너지 정제" +

                        "\n2. 데미지 타입 코어 억제[2] - 데미지 종류 인식에 이상이 감지됩니다." +
                        ((damageTypeCoreCleared) ? "<억제 완료>" : "<억제 미완료>") +
                        "\n- 보상 : 모든 직원들이 받는 데미지 10% 감소" +
                        "\n- 활성 조건 : Day 30 이상" +
                        "\n- 목표 : 클리포트 카운터 9 달성 및 에너지 정제" +

                        "\n3. PE 박스 식별 불가 코어 억제[3] - PE 박스 종류 인식에 이상이 감지됩니다." +
                        ((peCheckCoreCleared) ? "<억제 완료>" : "<억제 미완료>") +
                        "\n- 보상 : 모든 직원들의 이동속도 10% 증가" +
                        "\n- 활성 조건 : Day 30 이상" +
                        "\n- 목표 : 클리포트 카운터 8 달성 및 에너지 정제" +

                        "\n4. 공포 코어 억제[4] - 직원들의 공포에 이상이 감지됩니다." +
                        ((fearCoreCleared) ? "<억제 완료>" : "<억제 미완료>") +
                        "\n- 보상 : 모든 직원들이 받는 공포레벨이 3레벨 감소" +
                        "\n- 활성 조건 : Day 30 이상" +
                        "\n- 목표 : 클리포트 카운터 8 달성 및 에너지 정제" +

                        "\n5. 시련 폭주 코어 억제[5] - 당신은 죄악이 등을 타고 오르는 것을 느꼈다." +
                        ((ordealCoreCleared) ? "<억제 완료>" : "<억제 미완료>") +
                        "\n- 보상 : 모든 직원들의 공격력 30% 증가" +
                        "\n- 활성 조건 : Day 35 이상 및 1~4 코어 모두 억제" +
                        "\n- 목표 : 클리포트 카운터 10 달성 및 에너지 정제";
                    openDesc(desc, 0, -150, 1500, 1000);
                }
                else
                {
                    closeDesc();
                }
            }
            return true;
        }

        public static void openDesc(String desc,int posX,int posY,int scaleX,int scaleY)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = "NewCanvas";
            gameObj.AddComponent<Canvas>();

            Canvas gameCanvas = gameObj.GetComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            gameCanvas.worldCamera = Camera.main;
            gameCanvas.sortingOrder = 999;
            gameObj.AddComponent<CanvasScaler>();
            gameObj.AddComponent<GraphicRaycaster>();

            gameObj.transform.SetParent(Camera.main.transform);
            gameObj.transform.SetAsFirstSibling();


            GameObject gameObj2 = new GameObject("NewText");
            gameObj2.transform.SetParent(gameObj.transform);
            gameObj2.transform.localPosition = new Vector3(posX, posY, 0);
            gameObj2.transform.localScale = new Vector3(1, 1, 0);

            Text gameText = gameObj2.AddComponent<Text>();
            gameText.text = desc;

            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            gameText.font = ArialFont;
            gameText.material = ArialFont.material;
            gameText.fontSize = 30;

            gameObj2.GetComponent<RectTransform>().sizeDelta = new Vector2(scaleX, scaleY);

            description = gameObj;
            isOpen = true;
        }

        public static void closeDesc()
        {
            UnityEngine.Object.Destroy(description);
            isOpen = false;
        }

        public static bool checkToday(int day)
        {
            int currentDay = PlayerModel.instance.GetDay() + 1;

            if (currentDay >= day)
                return true;
            else
                return false;
        }

        public static void InitUseSkillAction(ref UseSkill __result)
        {
            if(speedCoreCleared) __result.workSpeed *= 1.25f;
            if(speedCh)
            {
                int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
                float MinusLevel = 1;
                if (level == 2 || level == 3)
                {
                    MinusLevel = UnityEngine.Random.Range(0.75f, 1f);
                }
                else if (level == 4 || level == 5)
                {
                    MinusLevel = UnityEngine.Random.Range(0.5f, 0.75f);
                }
                else if (level >= 6)
                {
                    MinusLevel = UnityEngine.Random.Range(0.25f, 0.5f);
                }
                __result.workSpeed *= MinusLevel;
            }
        }

        public static bool ClearStage_patch()
        {
            if (CoreCh)
            {
                float energy = EnergyModel.instance.GetEnergy();
                float needEnery = StageTypeInfo.instnace.GetEnergyNeed(PlayerModel.instance.GetDay());
                if (speedCh && energy >= needEnery && CreatureOverloadManager.instance.GetQliphothOverloadLevel() >= 7)
                {
                    speedCoreCleared = true;
                    SaveData();
                    return true;
                }
                else if (damageTypeCh && energy >= needEnery && CreatureOverloadManager.instance.GetQliphothOverloadLevel() >= 9)
                {
                    damageTypeCoreCleared = true;
                    SaveData();
                    return true;
                }
                else if (peCheckCoreCh && energy >= needEnery && CreatureOverloadManager.instance.GetQliphothOverloadLevel() >= 8)
                {
                    peCheckCoreCleared = true;
                    SaveData();
                    return true;
                }
                else if (fearCoreCh && energy >= needEnery && CreatureOverloadManager.instance.GetQliphothOverloadLevel() >= 8)
                {
                    fearCoreCleared = true;
                    SaveData();
                    return true;
                }
                else if (ordealCoreCh && energy >= needEnery && CreatureOverloadManager.instance.GetQliphothOverloadLevel() >= 10)
                {
                    ordealCoreCleared = true;
                    SaveData();
                    return true;
                }
                return false;
            }
            return true;
        }

        public static void SaveData()
        {
            String saveData = "";
            saveData += (speedCoreCleared ? "1" : "0");
            saveData += (damageTypeCoreCleared ? "1" : "0");
            saveData += (peCheckCoreCleared ? "1" : "0");
            saveData += (fearCoreCleared ? "1" : "0");
            saveData += (ordealCoreCleared ? "1" : "0");
            File.WriteAllText(filePath, saveData);
        }

        public static void LoadData()
        {
            String loadData = File.ReadAllText(filePath);
            speedCoreCleared = loadData[0] == '1';
            damageTypeCoreCleared = loadData[1] == '1';
            peCheckCoreCleared = loadData[2] == '1';
            fearCoreCleared = loadData[3] == '1';
            ordealCoreCleared = loadData[4] == '1';
        }

        public static void GetMovementValue_patch(ref float __result)
        {
            if(speedCh)
            {
                int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
                float MinusLevel = 1;
                if (level == 2 || level == 3)
                {
                    MinusLevel = UnityEngine.Random.Range(0.75f, 1f);
                }
                else if (level == 4 || level == 5)
                {
                    MinusLevel = UnityEngine.Random.Range(0.5f, 0.75f);
                }
                else if (level >= 6)
                {
                    MinusLevel = UnityEngine.Random.Range(0.25f, 0.5f);
                }
                __result *= MinusLevel;
            }
            if(peCheckCoreCleared)
            {
                __result *= 1.1f;
            }
        }

        public static bool OnTakeDamage_Patch(EquipmentModel __instance, ref UnitModel actor, ref DamageInfo dmg)
        {
            if (actor is CreatureModel)
            {
                if(damageTypeCh)
                {
                    int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
                    if(level == 2 || level == 3)
                    {
                        dmg.type = RwbpType.R;
                    }
                    else if (level == 4 || level == 5)
                    {
                        dmg.type = RwbpType.W;
                    }
                    else if (level == 6)
                    {
                        dmg.type = RwbpType.B;
                    }
                    else if (level >= 7)
                    {
                        dmg.type = RwbpType.P;
                    }
                    dmg *= 1.25f;
                }
                if(damageTypeCoreCleared)
                {
                    dmg *= 0.95f;
                }
                __instance.script.OnTakeDamage(actor, ref dmg);
            }
            else
            {
                if (ordealCoreCleared)
                    dmg *= 1.15f;
                __instance.script.OnTakeDamage(actor, ref dmg);
            }
            return false;
        }

        public static bool TakeDamageWithoutEffect_Patch(ref UnitModel actor, ref DamageInfo dmg)
        {
            if(damageTypeCh)
            {
                dmg *= 0.75f;
            }
            return true;
        }

        public static void playBGM(AudioClip clip)
        {
            if (BgmManager.instance.audioSource.clip == clip)
                return;
            if(BgmManager.instance.audioSource.isPlaying)
            {
                BgmManager.instance.audioSource.Stop();
            }
            BgmManager.instance.audioSource.clip = clip;
            BgmManager.instance.audioSource.loop = true;
            BgmManager.instance.audioSource.Play();
        }

        public static bool CheckLive_patch(UseSkill __instance)
        {
            if(peCheckCoreCh)
            {
                int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();

                bool goChange = false;
                float randRan = UnityEngine.Random.Range(0f, 1f);
                if ((level == 2 || level == 3) && randRan <= 0.3f)
                    goChange = true;
                else if ((level == 4 || level == 5) && randRan <= 0.6f)
                    goChange = true;
                else if (level >= 6)
                    goChange = true;

                if (__instance.workCount == __instance.maxCubeCount && goChange)
                {
                    int successC = __instance.successCount;
                    int failC = __instance.failCount;

                    __instance.successCount = failC;
                    __instance.failCount = successC;
                }
            }
            return true;
        }

        public static void GetRiskLevel_patch(CreatureModel __instance, ref int __result)
        {
            if(fearCoreCh)
            {
                if(__instance.metadataId != 100056 && __instance.metadataId != 100015)
                {
                    int level = CreatureOverloadManager.instance.GetQliphothOverloadLevel();
                    if (level == 2 || level == 3)
                    {
                        __result += 1;
                    }
                    else if (level == 4 || level == 5)
                    {
                        __result += 2;
                    }
                    else if (level >= 6)
                    {
                        __result += 3;
                    }
                }
            }
            if (fearCoreCleared)
                __result -= 3;
        }

        public static void loadUndertailMusic()
        {
            String musicPath = @"file://" + Application.dataPath + @"/BaseMods/NULL_MINICORE_MOD_FINAL/Megalovania.wav";

            WWW tmpWWW = new WWW(musicPath);
            while (!tmpWWW.isDone) ;
            AudioClip musicSrc = tmpWWW.GetAudioClip(false, false);
            playBGM(musicSrc);
        }

        public static bool isOpen = false;
        public static GameObject description;
        public static string filePath = @".\MiniCoreData.bin";

        public static bool CoreCh = false;
        public static int prNum = 0;

        public static bool speedCoreCleared = false;
        public static bool speedCh = false;
        public static MalkutBossBase malkutBase;

        public static bool damageTypeCoreCleared = false;
        public static bool damageTypeCh = false;
        public static NetzachBossBase netzachBase;

        public static bool peCheckCoreCleared = false;
        public static bool peCheckCoreCh = false;

        public static bool fearCoreCleared = false;
        public static bool fearCoreCh = false;

        public static bool ordealCoreCleared = false;
        public static bool ordealCoreCh = false;
    }
}
