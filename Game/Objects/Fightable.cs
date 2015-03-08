using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface Fightable
{
	string getName();
	int getDef();
	int getCurHP();
	short getCurMP();
	byte getLevel();
	int getuID();
	void recDamage(int uid, int amount);
	int getAtkSuc();
	int getDefSuc();
	int getCritRate();
	int getCritDmg();
}