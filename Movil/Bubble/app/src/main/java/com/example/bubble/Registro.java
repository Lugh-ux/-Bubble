package com.example.bubble;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

public class Registro extends AppCompatActivity {

    Button btRegistro;
    EditText txtUsuario, txtCorreo, txtContraseña, txtConfContraseña;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_registro);
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });

        TextView login = findViewById(R.id.txtIniciarsesion);
        login.setOnClickListener(v -> {
            startActivity(new Intent(this, LogIn.class));

        });

        btRegistro = findViewById(R.id.btnRegistro);
        txtUsuario = findViewById(R.id.createBoxUsuario);

        btRegistro.setOnClickListener(v -> {
            String nombre = txtUsuario.getText().toString();
            APIRest api = new APIRest();
            api.subirUsuario(nombre);
        });


    }
}