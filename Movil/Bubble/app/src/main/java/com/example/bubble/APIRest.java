package com.example.bubble;

import org.json.JSONObject;

import java.io.IOException;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.charset.StandardCharsets;

public class APIRest {

    //Prueba acabar despues
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
}
