package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataSystemPojo {
	private Integer systemId;
	private String systemName;
	private String help;
	private HashMap<String, Object> openCondition;
	private ArrayList<HashMap<String, Object>> openRewards;

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
	public HashMap<String, Object> getOpenCondition() {
		return openCondition;
	}

	public void setOpenCondition(HashMap<String, Object> openCondition) {
		this.openCondition = openCondition;
	}
	public ArrayList<HashMap<String, Object>> getOpenRewards() {
		return openRewards;
	}

	public void setOpenRewards(ArrayList<HashMap<String, Object>> openRewards) {
		this.openRewards = openRewards;
	}

	public TableDataSystemPojo() {
	}

	public TableDataSystemPojo(Integer systemId, String systemName, String help, HashMap<String, Object> openCondition, ArrayList<HashMap<String, Object>> openRewards) {
		this.systemId = systemId;
		this.systemName = systemName;
		this.help = help;
		this.openCondition = openCondition;
		this.openRewards = openRewards;
	}
}
