package com.example.bubble;

import android.app.Activity;
import android.graphics.Color;

import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.model.CircleOptions;
import com.google.android.gms.maps.model.LatLng;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.charset.StandardCharsets;

public class APIRest {


    public void subirUsuario(String nombre) {
        new Thread(() -> {

            try {
                URL url = new URL("http://192.130.0.154:8080/tema5maven/rest/deportistas/android");

                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("POST");
                con.setRequestProperty("Content-Type", "application/json");
                con.setDoOutput(true);

                JSONObject json = new JSONObject();
                json.put("nombre", nombre);

                System.out.println(json);

                try (OutputStream os = con.getOutputStream()) {
                    os.write(json.toString().getBytes(StandardCharsets.UTF_8));
                } catch (IOException e ){
                    throw new RuntimeException(e);
                }

                System.out.println(con.getResponseCode());


            } catch (Exception e) {
                throw new RuntimeException(e);
            }

    }).start();
    }

    public void subirBurbuja(double lat, double lng) {
        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/burbujas/add");

                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("POST");
                con.setRequestProperty("Content-Type", "application/json");
                con.setDoOutput(true);

                JSONObject json = new JSONObject();
                json.put("user_id", 1);
                json.put("latitude", lat);
                json.put("longitude", lng);

                try (OutputStream os = con.getOutputStream()) {
                    os.write(json.toString().getBytes(StandardCharsets.UTF_8));
                }

                int responseCode = con.getResponseCode();
                System.out.println("Respuesta del servidor: " + responseCode);

            } catch (Exception e) {
                e.printStackTrace();
            }
        }).start();
    }

    public void cargarBurbujasEnMapa(GoogleMap mapa, Activity actividad) {
        new Thread(() -> {
            try {
                URL url = new URL("http://10.0.2.2:8080/tema5maven/rest/burbujas");
                HttpURLConnection con = (HttpURLConnection) url.openConnection();
                con.setRequestMethod("GET");

                if (con.getResponseCode() == 200) {
                    java.util.Scanner s = new java.util.Scanner(con.getInputStream()).useDelimiter("\\A");
                    String result = s.hasNext() ? s.next() : "";

                    actividad.runOnUiThread(() -> {
                        try {
                            JSONArray array = new JSONArray(result);
                            for (int i = 0; i < array.length(); i++) {
                                JSONObject obj = array.getJSONObject(i);
                                LatLng pos = new LatLng(obj.getDouble("latitude"), obj.getDouble("longitude"));

                                mapa.addCircle(new CircleOptions()
                                        .center(pos)
                                        .radius(100)
                                        .strokeWidth(4)
                                        .strokeColor(Color.parseColor("#4285F4"))
                                        .fillColor(Color.parseColor("#404285F4")));
                            }
                        } catch (Exception e) {
                            e.printStackTrace();
                        }
                    });
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }).start();
    }
}
