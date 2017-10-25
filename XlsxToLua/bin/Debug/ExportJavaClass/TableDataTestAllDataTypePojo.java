package org.xlsxtolua.pojo;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Date;

public class TableDataTestAllDataTypePojo {
	private Integer id;
	private Boolean testBool;
	private String testLang;
	private Long testLong;
	private HashMap<String, Object> testSimpleDict;
	private HashMap<String, Object> testDictIncludeArray;
	private Float testFloat;
	private String testString;
	private ArrayList<String> testSimpleArray;
	private ArrayList<String> testSimpleArray2;
	private HashMap<Integer ,Boolean> testTableString1;
	private ArrayList<ArrayList<String>> testArrayIncludeArray;
	private ArrayList<Integer> testTableString2;
	private ArrayList<HashMap<String, Object>> testArrayIncludeDict;
	private HashMap<String, Object> testDictIncludeDict;
	private ArrayList<HashMap<String, Object>> testTableString3;
	private Date testDate1;
	private Date testDate2;
	private Date testDate3;
	private Date testDate4;
	private Date testTime1;
	private Date testTime2;
	private Date testTime3;
	private HashMap<String, Object> testExportJsonSpecialString;
	private Object testJson;

	public Integer getId() {
		return id;
	}

	public void setId(Integer id) {
		this.id = id;
	}
	public Boolean getTestBool() {
		return testBool;
	}

	public void setTestBool(Boolean testBool) {
		this.testBool = testBool;
	}
	public String getTestLang() {
		return testLang;
	}

	public void setTestLang(String testLang) {
		this.testLang = testLang;
	}
	public Long getTestLong() {
		return testLong;
	}

	public void setTestLong(Long testLong) {
		this.testLong = testLong;
	}
	public HashMap<String, Object> getTestSimpleDict() {
		return testSimpleDict;
	}

	public void setTestSimpleDict(HashMap<String, Object> testSimpleDict) {
		this.testSimpleDict = testSimpleDict;
	}
	public HashMap<String, Object> getTestDictIncludeArray() {
		return testDictIncludeArray;
	}

	public void setTestDictIncludeArray(HashMap<String, Object> testDictIncludeArray) {
		this.testDictIncludeArray = testDictIncludeArray;
	}
	public Float getTestFloat() {
		return testFloat;
	}

	public void setTestFloat(Float testFloat) {
		this.testFloat = testFloat;
	}
	public String getTestString() {
		return testString;
	}

	public void setTestString(String testString) {
		this.testString = testString;
	}
	public ArrayList<String> getTestSimpleArray() {
		return testSimpleArray;
	}

	public void setTestSimpleArray(ArrayList<String> testSimpleArray) {
		this.testSimpleArray = testSimpleArray;
	}
	public ArrayList<String> getTestSimpleArray2() {
		return testSimpleArray2;
	}

	public void setTestSimpleArray2(ArrayList<String> testSimpleArray2) {
		this.testSimpleArray2 = testSimpleArray2;
	}
	public HashMap<Integer ,Boolean> getTestTableString1() {
		return testTableString1;
	}

	public void setTestTableString1(HashMap<Integer ,Boolean> testTableString1) {
		this.testTableString1 = testTableString1;
	}
	public ArrayList<ArrayList<String>> getTestArrayIncludeArray() {
		return testArrayIncludeArray;
	}

	public void setTestArrayIncludeArray(ArrayList<ArrayList<String>> testArrayIncludeArray) {
		this.testArrayIncludeArray = testArrayIncludeArray;
	}
	public ArrayList<Integer> getTestTableString2() {
		return testTableString2;
	}

	public void setTestTableString2(ArrayList<Integer> testTableString2) {
		this.testTableString2 = testTableString2;
	}
	public ArrayList<HashMap<String, Object>> getTestArrayIncludeDict() {
		return testArrayIncludeDict;
	}

	public void setTestArrayIncludeDict(ArrayList<HashMap<String, Object>> testArrayIncludeDict) {
		this.testArrayIncludeDict = testArrayIncludeDict;
	}
	public HashMap<String, Object> getTestDictIncludeDict() {
		return testDictIncludeDict;
	}

	public void setTestDictIncludeDict(HashMap<String, Object> testDictIncludeDict) {
		this.testDictIncludeDict = testDictIncludeDict;
	}
	public ArrayList<HashMap<String, Object>> getTestTableString3() {
		return testTableString3;
	}

	public void setTestTableString3(ArrayList<HashMap<String, Object>> testTableString3) {
		this.testTableString3 = testTableString3;
	}
	public Date getTestDate1() {
		return testDate1;
	}

	public void setTestDate1(Date testDate1) {
		this.testDate1 = testDate1;
	}
	public Date getTestDate2() {
		return testDate2;
	}

	public void setTestDate2(Date testDate2) {
		this.testDate2 = testDate2;
	}
	public Date getTestDate3() {
		return testDate3;
	}

	public void setTestDate3(Date testDate3) {
		this.testDate3 = testDate3;
	}
	public Date getTestDate4() {
		return testDate4;
	}

	public void setTestDate4(Date testDate4) {
		this.testDate4 = testDate4;
	}
	public Date getTestTime1() {
		return testTime1;
	}

	public void setTestTime1(Date testTime1) {
		this.testTime1 = testTime1;
	}
	public Date getTestTime2() {
		return testTime2;
	}

	public void setTestTime2(Date testTime2) {
		this.testTime2 = testTime2;
	}
	public Date getTestTime3() {
		return testTime3;
	}

	public void setTestTime3(Date testTime3) {
		this.testTime3 = testTime3;
	}
	public HashMap<String, Object> getTestExportJsonSpecialString() {
		return testExportJsonSpecialString;
	}

	public void setTestExportJsonSpecialString(HashMap<String, Object> testExportJsonSpecialString) {
		this.testExportJsonSpecialString = testExportJsonSpecialString;
	}
	public Object getTestJson() {
		return testJson;
	}

	public void setTestJson(Object testJson) {
		this.testJson = testJson;
	}

	public TableDataTestAllDataTypePojo() {
	}

	public TableDataTestAllDataTypePojo(Integer id, Boolean testBool, String testLang, Long testLong, HashMap<String, Object> testSimpleDict, HashMap<String, Object> testDictIncludeArray, Float testFloat, String testString, ArrayList<String> testSimpleArray, ArrayList<String> testSimpleArray2, HashMap<Integer ,Boolean> testTableString1, ArrayList<ArrayList<String>> testArrayIncludeArray, ArrayList<Integer> testTableString2, ArrayList<HashMap<String, Object>> testArrayIncludeDict, HashMap<String, Object> testDictIncludeDict, ArrayList<HashMap<String, Object>> testTableString3, Date testDate1, Date testDate2, Date testDate3, Date testDate4, Date testTime1, Date testTime2, Date testTime3, HashMap<String, Object> testExportJsonSpecialString, Object testJson) {
		this.id = id;
		this.testBool = testBool;
		this.testLang = testLang;
		this.testLong = testLong;
		this.testSimpleDict = testSimpleDict;
		this.testDictIncludeArray = testDictIncludeArray;
		this.testFloat = testFloat;
		this.testString = testString;
		this.testSimpleArray = testSimpleArray;
		this.testSimpleArray2 = testSimpleArray2;
		this.testTableString1 = testTableString1;
		this.testArrayIncludeArray = testArrayIncludeArray;
		this.testTableString2 = testTableString2;
		this.testArrayIncludeDict = testArrayIncludeDict;
		this.testDictIncludeDict = testDictIncludeDict;
		this.testTableString3 = testTableString3;
		this.testDate1 = testDate1;
		this.testDate2 = testDate2;
		this.testDate3 = testDate3;
		this.testDate4 = testDate4;
		this.testTime1 = testTime1;
		this.testTime2 = testTime2;
		this.testTime3 = testTime3;
		this.testExportJsonSpecialString = testExportJsonSpecialString;
		this.testJson = testJson;
	}
}
