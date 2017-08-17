package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataHeroEquipmentPojo {
	private Integer id;
	private Integer heroId;
	private Integer heroQuality;
	private Integer seq;
	private Integer propId;
	private Integer equipRank;

	public Integer getId() {
		return id;
	}

	public void setId(Integer id) {
		this.id = id;
	}
	public Integer getHeroId() {
		return heroId;
	}

	public void setHeroId(Integer heroId) {
		this.heroId = heroId;
	}
	public Integer getHeroQuality() {
		return heroQuality;
	}

	public void setHeroQuality(Integer heroQuality) {
		this.heroQuality = heroQuality;
	}
	public Integer getSeq() {
		return seq;
	}

	public void setSeq(Integer seq) {
		this.seq = seq;
	}
	public Integer getPropId() {
		return propId;
	}

	public void setPropId(Integer propId) {
		this.propId = propId;
	}
	public Integer getEquipRank() {
		return equipRank;
	}

	public void setEquipRank(Integer equipRank) {
		this.equipRank = equipRank;
	}

	public TableDataHeroEquipmentPojo() {
	}

	public TableDataHeroEquipmentPojo(Integer id, Integer heroId, Integer heroQuality, Integer seq, Integer propId, Integer equipRank) {
		this.id = id;
		this.heroId = heroId;
		this.heroQuality = heroQuality;
		this.seq = seq;
		this.propId = propId;
		this.equipRank = equipRank;
	}
}
