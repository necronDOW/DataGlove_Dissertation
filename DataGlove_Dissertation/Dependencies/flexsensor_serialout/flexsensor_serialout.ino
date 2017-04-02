const int sensorCount = 4;
const float vcc = 4.98;
const float rDiv = 20000.0;

void setup() {
  Serial.begin(9600);
}

void loop() {
  for (int i = 0; i < sensorCount; i++) {
    Serial.print(String(GetR(i)) + ",");
  }
  
  Serial.println();
}

float GetR(int pinIndex) {
  int flexADC = analogRead(A0 + pinIndex);
  float flexV = flexADC * vcc / 1023.0;
  return rDiv * (vcc / flexV - 1.0);
}

