package ej2.webservices;
import java.util.Collections;

import javax.servlet.MultipartConfigElement;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import org.springframework.util.unit.DataSize;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.boot.web.servlet.MultipartConfigFactory;
import org.springframework.boot.web.servlet.support.SpringBootServletInitializer;
// Refer the licensing package
import com.syncfusion.licensing.*;

// Register Syncfusion license 
SyncfusionLicenseProvider.registerLicense("YOUR LICENSE KEY"); 

@SpringBootApplication
public class RestServiceCorsApplication extends SpringBootServletInitializer {

	@Override
	protected SpringApplicationBuilder configure(SpringApplicationBuilder application) {
		return application.sources(RestServiceCorsApplication.class);
	}

	public static void main(String[] args) {
		SpringApplication app = new SpringApplication(RestServiceCorsApplication.class);
		app.setDefaultProperties(Collections.singletonMap("server.port", "9090"));
		app.run(args);
	}
	
	@Bean
	public MultipartConfigElement multipartConfigElement() {
	    MultipartConfigFactory factory = new MultipartConfigFactory();
	    factory.setMaxFileSize(DataSize.ofBytes(1073741824));
	    factory.setMaxRequestSize(DataSize.ofBytes(1073741824));
	    return factory.createMultipartConfig();
	}

	// @Bean
	// public WebMvcConfigurer corsConfigurer() {
	// return new WebMvcConfigurer() {
	// @Override
	// public void addCorsMappings(CorsRegistry registry) {
	// registry.addMapping("/greeting-javaconfig").allowedOrigins("http://localhost:9000");
	// }
	// };
	// }

}
