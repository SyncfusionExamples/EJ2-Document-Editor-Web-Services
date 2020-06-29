package ej2.webservices;

public class CustomRestrictParameter {
	public String passwordBase64;
	public String saltBase64;
	public int spinCount;

	public String getPasswordBase64() {
		return passwordBase64;
	}

	public String getSaltBase64() {
		return saltBase64;
	}

	public int getSpinCount() {
		return spinCount;
	}

	public void setPasswordBase64(String value) {
		passwordBase64= value;
	}

	public void setSaltBase64(String value) {
		saltBase64= value;
	}

	public void setSpinCount(int value) {
		spinCount= value;
	}
}
