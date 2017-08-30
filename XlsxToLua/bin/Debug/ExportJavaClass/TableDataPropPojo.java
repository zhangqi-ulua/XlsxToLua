package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataPropPojo {
	private Integer id;
	private Integer type;
	private Integer subType;
	private String name;
	private String desc;
	private Integer quality;
	private String icon;
	private Integer sellPrice;

	public Integer getId() {
		return id;
	}

	public void setId(Integer id) {
		this.id = id;
	}
	public Integer getType() {
		return type;
	}

	public void setType(Integer type) {
		this.type = type;
	}
	public Integer getSubType() {
		return subType;
	}

	public void setSubType(Integer subType) {
		this.subType = subType;
	}
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
	public String getDesc() {
		return desc;
	}

	public void setDesc(String desc) {
		this.desc = desc;
	}
	public Integer getQuality() {
		return quality;
	}

	public void setQuality(Integer quality) {
		this.quality = quality;
	}
	public String getIcon() {
		return icon;
	}

	public void setIcon(String icon) {
		this.icon = icon;
	}
	public Integer getSellPrice() {
		return sellPrice;
	}

	public void setSellPrice(Integer sellPrice) {
		this.sellPrice = sellPrice;
	}

	public TableDataPropPojo() {
	}

	public TableDataPropPojo(Integer id, Integer type, Integer subType, String name, String desc, Integer quality, String icon, Integer sellPrice) {
		this.id = id;
		this.type = type;
		this.subType = subType;
		this.name = name;
		this.desc = desc;
		this.quality = quality;
		this.icon = icon;
		this.sellPrice = sellPrice;
	}
}
