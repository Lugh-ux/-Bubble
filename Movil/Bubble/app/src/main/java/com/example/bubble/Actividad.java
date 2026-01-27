package com.example.bubble;

public class Actividad {
    private String nombreUsuario;
    private String accion;
    private String distancia;
    private int imagenPerfil;

    public String getNombreUsuario() {
        return nombreUsuario;
    }

    public String getAccion() {
        return accion;
    }

    public String getDistancia() {
        return distancia;
    }

    public int getImagenPerfil() {
        return imagenPerfil;
    }

    public Actividad(String nombreUsuario, String accion, String distancia, int imagenPerfil) {
        this.nombreUsuario = nombreUsuario;
        this.accion = accion;
        this.distancia = distancia;
        this.imagenPerfil = imagenPerfil;
    }

}
