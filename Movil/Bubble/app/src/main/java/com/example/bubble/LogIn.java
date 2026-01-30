package com.example.bubble;

import android.annotation.SuppressLint;
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

public class LogIn extends AppCompatActivity {

    @SuppressLint("MissingInflatedId")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_main);
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });

        Button btnLogin = findViewById(R.id.btnLogin);
        EditText txtEmail = findViewById(R.id.loginBoxUsuario);
        EditText txtContraseña = findViewById(R.id.loginBoxContraseña);
        TextView crear = findViewById(R.id.txtIniciarsesion);
        crear.setOnClickListener(v -> {
            startActivity(new Intent(this, Registro.class));

        });

        APIRest api = new APIRest();

        btnLogin.setOnClickListener(v -> {
            String email = txtEmail.getText().toString();
            String pass = txtContraseña.getText().toString();

            if (!email.isEmpty() && !pass.isEmpty()) {

                api.login(email, pass, this);
            } else {
                android.widget.Toast.makeText(this, "Rellena todos los campos", android.widget.Toast.LENGTH_SHORT).show();
            }
        });



    }

}