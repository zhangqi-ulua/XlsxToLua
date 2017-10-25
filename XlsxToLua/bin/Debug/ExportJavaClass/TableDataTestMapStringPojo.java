package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataTestMapStringPojo {
	private Integer id;
	private Integer type;
	private Object params;

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
	public Object getParams() {
		return params;
	}

	public void setParams(Object params) {
		this.params = params;
	}

	public TableDataTestMapStringPojo() {
	}

	public TableDataTestMapStringPojo(Integer id, Integer type, Object params) {
		this.id = id;
		this.type = type;
		this.params = params;
	}
}
