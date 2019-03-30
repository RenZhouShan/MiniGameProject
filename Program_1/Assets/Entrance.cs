using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class Entrance : MonoBehaviour
{
    LuaEnv luaenv = null;
    private void Awake()
    {
        luaenv = new LuaEnv();
        luaenv.DoString(@"require 'Zlua.Entrance'");
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
