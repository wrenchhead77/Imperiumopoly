using UnityEngine;

public class RulesManager : MonoBehaviour
{
    public soRuleMenu so_RuleMenu;
    public static RulesManager Instance;
    public Rule[] rules;
    public int Difficulty = 0;
    public int Sleight = 0;

    public const int maxDropDowns = 9;

    private void Awake()
    {
        Instance = this;

    }
    public void CreateRule()
    {
        GameMgr gm = GameMgr.Instance;
        soRuleMenu[] ruleMenu = gm.so_Ref.ruleMenus;

        rules = new Rule[maxDropDowns];

        for (int i = 0; i < maxDropDowns; i++)
        {
            rules[i] = new GameObject().AddComponent<Rule>();
            rules[i].transform.SetParent(this.transform);
        }
    }
}
