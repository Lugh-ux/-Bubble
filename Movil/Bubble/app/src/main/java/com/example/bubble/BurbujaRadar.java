package com.example.bubble;

public class BurbujaRadar {
    private String mensaje;
    private String distancia;

    public BurbujaRadar(String mensaje, String distancia) {
        this.mensaje = mensaje;
        this.distancia = distancia;
    }

    public String getMensaje() { return mensaje; }
    public String getDistancia() { return distancia; }
}