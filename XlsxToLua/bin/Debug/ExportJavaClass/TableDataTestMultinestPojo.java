package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;

public class TableDataTestMultinestPojo {
	private Integer id;
	private HashMap<String, Object> testDict;
	private ArrayList<HashMap<String, HashMap<String, Object>>> testArray1;
	private ArrayList<ArrayList<HashMap<String, Object>>> testArray2;

	public Integer getId() {
		return id;
	}

	public void setId(Integer id) {
		this.id = id;
	}
	public HashMap<String, Object> getTestDict() {
		return testDict;
	}

	public void setTestDict(HashMap<String, Object> testDict) {
		this.testDict = testDict;
	}
	public ArrayList<HashMap<String, HashMap<String, Object>>> getTestArray1() {
		return testArray1;
	}

	public void setTestArray1(ArrayList<HashMap<String, HashMap<String, Object>>> testArray1) {
		this.testArray1 = testArray1;
	}
	public ArrayList<ArrayList<HashMap<String, Object>>> getTestArray2() {
		return testArray2;
	}

	public void setTestArray2(ArrayList<ArrayList<HashMap<String, Object>>> testArray2) {
		this.testArray2 = testArray2;
	}

	public TableDataTestMultinestPojo() {
	}

	public TableDataTestMultinestPojo(Integer id, HashMap<String, Object> testDict, ArrayList<HashMap<String, HashMap<String, Object>>> testArray1, ArrayList<ArrayList<HashMap<String, Object>>> testArray2) {
		this.id = id;
		this.testDict = testDict;
		this.testArray1 = testArray1;
		this.testArray2 = testArray2;
	}
}
