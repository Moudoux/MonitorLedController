#include <FastLED.h>

#define LED_PIN 9
#define NUM_LEDS 30
#define DELAY 30
#define BRIGHTNESS 50

CRGB leds[NUM_LEDS];

void setup() { 
  Serial.begin(9600);
  randomSeed(analogRead(0));
  FastLED.addLeds<WS2812, LED_PIN, GRB>(leds, NUM_LEDS);
  FastLED.setBrightness(BRIGHTNESS);
  Serial.println("Ready");
}

int led = 0, h = 0;
bool state = true, state_hue = true, enabled = true;

void loop() {
  while (Serial.available() > 0) { 
    String cmd = Serial.readString();
     Serial.print("Got command: ");
     Serial.println(cmd);
     if (cmd == "led off") {
      Serial.println("Turning off the leds");
      for (int i = 0; i < NUM_LEDS; i++) {
        leds[i] = CRGB(0,0,0);
      }
      FastLED.show();
      enabled = false;
     } else if (cmd == "led on") {
      Serial.println("Turning on the leds");
      enabled = true;
     } else {
      Serial.println("Unknown command");
     }
  }
  if (h == 360) {
    h = 0;
  }
  h++;
  if (enabled) {
    setHue(h);
  }
}

void setHue(int hue) {
    leds[led].setHue(hue);
    FastLED.show();
    delay(DELAY);
    if (state) {
      led++;
      if (led == (NUM_LEDS - 1)) {
        state = false;
      }
    } else {
      led--;
      if (led == 0) {
        state = true;
      }
    }
}


