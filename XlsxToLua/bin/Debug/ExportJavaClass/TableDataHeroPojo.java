package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataHeroPojo {
	private Integer heroId;
	private String name;
	private Integer rare;
	private Integer type;
	private Integer defaultStar;
	private Boolean isOpen;
	private Object attributes;

	public Integer getHeroId() {
		return heroId;
	}

	public void setHeroId(Integer heroId) {
		this.heroId = heroId;
	}
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}
	public Integer getRare() {
		return rare;
	}

	public void setRare(Integer rare) {
		this.rare = rare;
	}
	public Integer getType() {
		return type;
	}

	public void setType(Integer type) {
		this.type = type;
	}
	public Integer getDefaultStar() {
		return defaultStar;
	}

	public void setDefaultStar(Integer defaultStar) {
		this.defaultStar = defaultStar;
	}
	public Boolean getIsOpen() {
		return isOpen;
	}

	public void setIsOpen(Boolean isOpen) {
		this.isOpen = isOpen;
	}
	public Object getAttributes() {
		return attributes;
	}

	public void setAttributes(Object attributes) {
		this.attributes = attributes;
	}

	public TableDataHeroPojo() {
	}

	public TableDataHeroPojo(Integer heroId, String name, Integer rare, Integer type, Integer defaultStar, Boolean isOpen, Object attributes) {
		this.heroId = heroId;
		this.name = name;
		this.rare = rare;
		this.type = type;
		this.defaultStar = defaultStar;
		this.isOpen = isOpen;
		this.attributes = attributes;
	}
}
