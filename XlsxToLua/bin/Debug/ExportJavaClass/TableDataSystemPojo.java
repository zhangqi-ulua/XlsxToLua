package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataSystemPojo {
	private Integer systemId;
	private String systemName;
	private String help;
	private HashMap<String, Integer> openCondition;
	private ArrayList<HashMap<String, Integer>> openRewards;

	public Integer getSystemId() {
		return systemId;
	}

	public void setSystemId(Integer systemId) {
		this.systemId = systemId;
	}
	public String getSystemName() {
		return systemName;
	}

	public void setSystemName(String systemName) {
		this.systemName = systemName;
	}
	public String getHelp() {
		return help;
	}

	public void setHelp(String help) {
		this.help = help;
	}
	public HashMap<String, Integer> getOpenCondition() {
		return openCondition;
	}

	public void setOpenCondition(HashMap<String, Integer> openCondition) {
		this.openCondition = openCondition;
	}
	public ArrayList<HashMap<String, Integer>> getOpenRewards() {
		return openRewards;
	}

	public void setOpenRewards(ArrayList<HashMap<String, Integer>> openRewards) {
		this.openRewards = openRewards;
	}

	public TableDataSystemPojo() {
	}

	public TableDataSystemPojo(Integer systemId, String systemName, String help, HashMap<String, Integer> openCondition, ArrayList<HashMap<String, Integer>> openRewards) {
		this.systemId = systemId;
		this.systemName = systemName;
		this.help = help;
		this.openCondition = openCondition;
		this.openRewards = openRewards;
	}
}
